using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Repositories;

namespace NFLDepthCharts.Tests.RepositoryTests
{
    [TestFixture]
    public class DepthChartRepositoryTests
    {
        private DepthChartDbContext _context;
        private IDepthChartRepository _repository;
        private Mock<ILogger<IDepthChartRepository>> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DepthChartDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DepthChartDbContext(options);
            _logger = new Mock<ILogger<IDepthChartRepository>>();
            _repository = new DepthChartRepository(_context, _logger.Object);

            // Seed the database
            var qb = new Position { PositionId = 1, Name = "QB" };
            var wr = new Position { PositionId = 2, Name = "WR" };

            _context.Positions.AddRange(qb, wr);

            var brady = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var edelman = new Player { PlayerId = 2, Number = 11, Name = "Julian Edelman" };

            _context.Players.AddRange(brady, edelman);
            _context.DepthChartEntries.AddRange(
                new DepthChartEntry { DepthChartEntryId = 1, PlayerId = 1, PositionId = 1, DepthLevel = 0 },
                new DepthChartEntry { DepthChartEntryId = 2, PlayerId = 2, PositionId = 2, DepthLevel = 0 }
            );
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetMaxDepthForPositionAsync_ExistingPosition_ReturnsMaxDepth()
        {
            var maxDepth = await _repository.GetMaxDepthForPositionAsync(1);

            Assert.That(maxDepth, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEntriesByPositionAsync_ExistingPosition_ReturnsEntries()
        {
            var entries = await _repository.GetEntriesByPositionAsync(1);

            Assert.That(entries.Count(), Is.EqualTo(1));
            Assert.That(entries.First().Player.Name, Is.EqualTo("Tom Brady"));
        }

        [Test]
        public async Task AddEntryAsync_NewEntry_AddsEntryAndReturnsIt()
        {
            var newEntry = new DepthChartEntry { PlayerId = 1, PositionId = 2, DepthLevel = 1 };

            await _repository.AddEntryAsync(newEntry);

            var entryInDb = await _context.DepthChartEntries.Where(d => d.PlayerId == 1 && d.PositionId == 2 && d.DepthLevel == 1).FirstOrDefaultAsync();
            Assert.That(entryInDb, Is.Not.Null);
            Assert.That(entryInDb.DepthChartEntryId, Is.GreaterThan(0));
            Assert.That(entryInDb.PositionId, Is.EqualTo(2));
        }

        [Test]
        public async Task RemoveEntryAsync_ExistingEntry_RemovesEntryAndReturnsIt()
        {
            var removedEntry = await _repository.RemoveEntryAsync(1, 1);

            Assert.That(removedEntry, Is.Not.Null);
            Assert.That(removedEntry.Player.Name, Is.EqualTo("Tom Brady"));

            var entryInDb = await _context.DepthChartEntries.FindAsync(1);
            Assert.That(entryInDb, Is.Null);
        }

        [Test]
        public async Task GetBackupsAsync_ExistingPlayerAndPosition_ReturnsBackups()
        {
            var backups = await _repository.GetBackupsAsync(1, 1);

            Assert.That(backups, Is.Empty);
        }

        [Test]
        public async Task GetFullDepthChartAsync_ReturnsFullDepthChart()
        {
            var fullDepthChart = await _repository.GetFullDepthChartAsync();

            Assert.That(fullDepthChart.Count, Is.EqualTo(2)); // QB and WR
            Assert.That(fullDepthChart["QB"].Count(), Is.EqualTo(1));
            Assert.That(fullDepthChart["WR"].Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetMaxDepthForPositionAsync_ExistingPosition_ReturnsMaxDepthWithMultipleEntries()
        {
            _context.DepthChartEntries.AddRange(
                new DepthChartEntry { DepthChartEntryId = 3, PlayerId = 1, PositionId = 1, DepthLevel = 1 },
                new DepthChartEntry { DepthChartEntryId = 4, PlayerId = 2, PositionId = 1, DepthLevel = 2 }
            );
            _context.SaveChanges();

            var maxDepth = await _repository.GetMaxDepthForPositionAsync(1);

            Assert.That(maxDepth, Is.EqualTo(2));
        }
    }
}
