using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Repositories;

namespace NFLDepthCharts.Tests.RepositoryTests
{
    [TestFixture]
    public class PositionRepositoryTests
    {
        private DepthChartDbContext _context;
        private IPositionRepository _repository;
        private Mock<ILogger<IPositionRepository>> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DepthChartDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DepthChartDbContext(options);
            _logger = new Mock<ILogger<IPositionRepository>>();
            _repository = new PositionRepository(_context, _logger.Object);

            // Seed the database
            _context.Positions.Add(new Position { PositionId = 1, Name = "QB" });
            _context.Positions.Add(new Position { PositionId = 2, Name = "WR" });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetByNameAsync_ExistingPosition_ReturnsPosition()
        {
            var position = await _repository.GetByNameAsync("QB");

            Assert.That(position, Is.Not.Null);
            Assert.That(position.Name, Is.EqualTo("QB"));
        }

        [Test]
        public async Task GetByNameAsync_NonExistingPosition_ReturnsNull()
        {
            var position = await _repository.GetByNameAsync("K");

            Assert.That(position, Is.Null);
        }
    }
}
