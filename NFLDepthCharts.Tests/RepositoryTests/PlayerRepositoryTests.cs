using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Repositories;

namespace NFLDepthCharts.Tests.RepositoryTests
{
    [TestFixture]
    public class PlayerRepositoryTests
    {
        private DepthChartDbContext _context;
        private IPlayerRepository _repository;
        private Mock<ILogger<IPlayerRepository>> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DepthChartDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DepthChartDbContext(options);
            _logger = new Mock<ILogger<IPlayerRepository>>();
            _repository = new PlayerRepository(_context, _logger.Object);

            _context.Players.Add(new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" });
            _context.Players.Add(new Player { PlayerId = 2, Number = 87, Name = "Rob Gronkowski" });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetByNumberAsync_ExistingPlayer_ReturnsPlayer()
        {
            var player = await _repository.GetByNumberAsync(12);

            Assert.That(player, Is.Not.Null);
            Assert.That(player.Name, Is.EqualTo("Tom Brady"));
        }

        [Test]
        public async Task GetByNumberAsync_NonExistingPlayer_ReturnsNull()
        {
            var player = await _repository.GetByNumberAsync(99);

            Assert.That(player, Is.Null);
        }

        [Test]
        public async Task AddAsync_NewPlayer_AddsPlayerAndReturnsItWithId()
        {
            var newPlayer = new Player { Number = 11, Name = "Julian Edelman" };

            var addedPlayer = await _repository.AddAsync(newPlayer);

            Assert.That(addedPlayer.PlayerId, Is.GreaterThan(0));
            Assert.That(addedPlayer.Name, Is.EqualTo("Julian Edelman"));

            var playerInDb = await _context.Players.FindAsync(addedPlayer.PlayerId);
            Assert.That(playerInDb, Is.Not.Null);
            Assert.That(playerInDb.PlayerId, Is.GreaterThan(0));
        }
    }

}
