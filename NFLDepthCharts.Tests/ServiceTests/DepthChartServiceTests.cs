using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Repositories;
using NFLDepthCharts.API.Services;
using NFLDepthCharts.API.Validators;

namespace NFLDepthCharts.Tests.ServiceTests
{
    [TestFixture]
    public class DepthChartServiceTests
    {
        private ServiceProvider _serviceProvider;
        private Mock<IPlayerRepository> _mockPlayerRepo;
        private Mock<IPositionRepository> _mockPositionRepo;
        private Mock<IDepthChartRepository> _mockDepthChartRepo;
        private Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();

            _mockPlayerRepo = new Mock<IPlayerRepository>();
            _mockPositionRepo = new Mock<IPositionRepository>();
            _mockDepthChartRepo = new Mock<IDepthChartRepository>();
            _mockMapper = new Mock<IMapper>();

            serviceCollection.AddSingleton(_mockPlayerRepo.Object);
            serviceCollection.AddSingleton(_mockPositionRepo.Object);
            serviceCollection.AddSingleton(_mockDepthChartRepo.Object);
            serviceCollection.AddSingleton(_mockMapper.Object);

            // Using autofac would have avoided this
            serviceCollection.AddTransient<IAddPlayerToDepthChartDtoValidator, AddPlayerToDepthChartDtoValidator>();
            serviceCollection.AddTransient<IDepthChartEntryValidator, DepthChartEntryValidator>();
            serviceCollection.AddTransient<IPlayerValidator, PlayerValidator>();
            serviceCollection.AddTransient<IPositionValidator, PositionValidator>();
            serviceCollection.AddTransient<IDepthChartService, DepthChartService>();
            serviceCollection.AddLogging();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        [Test]
        public async Task AddPlayerToDepthChart_ValidInput_ReturnsTrue()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "Tom Brady" };
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync((Player)null);
            _mockMapper.Setup(m => m.Map<Player>(It.IsAny<AddPlayerToDepthChartDto>())).Returns(player);
            _mockPlayerRepo.Setup(r => r.AddAsync(It.IsAny<Player>())).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetMaxDepthForPositionAsync(1)).ReturnsAsync(0);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(new List<DepthChartEntry>());
            _mockDepthChartRepo.Setup(r => r.AddEntryAsync(It.IsAny<DepthChartEntry>())).ReturnsAsync(new DepthChartEntry());

            // Act
            var result = await service.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AddPlayerToDepthChart_ExistingPlayer_UpdatesDepthChart()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "Tom Brady" };
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetMaxDepthForPositionAsync(1)).ReturnsAsync(1);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(new List<DepthChartEntry>());
            _mockDepthChartRepo.Setup(r => r.AddEntryAsync(It.IsAny<DepthChartEntry>())).ReturnsAsync(new DepthChartEntry());

            // Act
            var result = await service.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void AddPlayerToDepthChart_PositionNotFound_ThrowsValidationException()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "InvalidPosition", PlayerNumber = 12, PlayerName = "Tom Brady" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("InvalidPosition")).ReturnsAsync((Position)null);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => service.AddPlayerToDepthChart(dto));
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_ExistingPlayer_ReturnsPlayerDto()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var playerDto = new PlayerDto { Number = 12, Name = "Tom Brady" };
            var depthChartEntry = new DepthChartEntry { DepthChartEntryId = 1, PlayerId = 1, PositionId = 1, DepthLevel = 0, Player = player };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.RemoveEntryAsync(1, 1)).ReturnsAsync(depthChartEntry);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(new List<DepthChartEntry>());
            _mockMapper.Setup(m => m.Map<PlayerDto>(It.IsAny<Player>())).Returns(playerDto);

            // Act
            var result = await service.RemovePlayerFromDepthChart("QB", 12);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Number, Is.EqualTo(12));
            Assert.That(result.Name, Is.EqualTo("Tom Brady"));
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_PlayerNotFound_ThrowsValidationException()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync((Player)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(
                () => service.RemovePlayerFromDepthChart("QB", 12));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNotFound));
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_PlayerNotInDepthChart_ThrowsValidationException()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.RemoveEntryAsync(1, 1)).ReturnsAsync((DepthChartEntry)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(
                () => service.RemovePlayerFromDepthChart("QB", 12));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNotInDepthChart));
        }

        [Test]
        public async Task GetBackups_ExistingPlayerAndPosition_ReturnsBackups()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var backupPlayer1 = new Player { PlayerId = 2, Number = 10, Name = "Jimmy Garoppolo" };
            var backupPlayer2 = new Player { PlayerId = 3, Number = 5, Name = "Brian Hoyer" };
            var backupEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { DepthChartEntryId = 2, PlayerId = 2, PositionId = 1, DepthLevel = 1, Player = backupPlayer1 },
                new DepthChartEntry { DepthChartEntryId = 3, PlayerId = 3, PositionId = 1, DepthLevel = 2, Player = backupPlayer2 }
            };
            var backupPlayerDtos = new List<PlayerDto>
            {
                new PlayerDto { Number = 10, Name = "Jimmy Garoppolo" },
                new PlayerDto { Number = 5, Name = "Brian Hoyer" }
            };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetBackupsAsync(1, 1)).ReturnsAsync(backupEntries);
            _mockMapper.Setup(m => m.Map<IEnumerable<PlayerDto>>(It.IsAny<IEnumerable<Player>>()))
                       .Returns(backupPlayerDtos);

            // Act
            var result = await service.GetBackups("QB", 12);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Jimmy Garoppolo"));
            Assert.That(result.Last().Name, Is.EqualTo("Brian Hoyer"));
        }

        [Test]
        public async Task GetBackups_PlayerNotFound_ThrowsValidationException()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync((Player)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() => service.GetBackups("QB", 12));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNotFound));
        }

        [Test]
        public async Task GetBackups_PositionNotFound_ThrowsValidationException()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();

            _mockPositionRepo.Setup(r => r.GetByNameAsync("InvalidPosition")).ReturnsAsync((Position)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() => service.GetBackups("InvalidPosition", 12));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionNotFound));
        }

        [Test]
        public async Task GetFullDepthChart_ReturnsFullDepthChart()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var depthChart = new Dictionary<string, IEnumerable<Player>>
            {
                { "QB", new List<Player> { new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" } } },
                { "WR", new List<Player> { new Player { PlayerId = 2, Number = 11, Name = "Julian Edelman" } } }
            };
            var fullDepthChartDto = new FullDepthChartDto
            {
                Positions = new Dictionary<string, List<PlayerDto>>
                {
                    { "QB", new List<PlayerDto> { new PlayerDto { Number = 12, Name = "Tom Brady" } } },
                    { "WR", new List<PlayerDto> { new PlayerDto { Number = 11, Name = "Julian Edelman" } } }
                }
            };

            _mockDepthChartRepo.Setup(r => r.GetFullDepthChartAsync()).ReturnsAsync(depthChart);
            _mockMapper.Setup(m => m.Map<FullDepthChartDto>(It.IsAny<Dictionary<string, IEnumerable<Player>>>())).Returns(fullDepthChartDto);

            // Act
            var result = await service.GetFullDepthChart();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Positions.Count, Is.EqualTo(2));
            Assert.That(result.Positions["QB"].First().Name, Is.EqualTo("Tom Brady"));
            Assert.That(result.Positions["WR"].First().Name, Is.EqualTo("Julian Edelman"));
        }

        [Test]
        public async Task GetFullDepthChart_EmptyDepthChart_ReturnsEmptyDictionary()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var emptyDepthChart = new Dictionary<string, IEnumerable<Player>>();
            var emptyFullDepthChartDto = new FullDepthChartDto { Positions = new Dictionary<string, List<PlayerDto>>() };

            _mockDepthChartRepo.Setup(r => r.GetFullDepthChartAsync()).ReturnsAsync(emptyDepthChart);
            _mockMapper.Setup(m => m.Map<FullDepthChartDto>(It.IsAny<Dictionary<string, IEnumerable<Player>>>())).Returns(emptyFullDepthChartDto);

            // Act
            var result = await service.GetFullDepthChart();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Positions, Is.Empty);
        }

        [Test]
        public void AddPlayerToDepthChart_InvalidInput_ThrowsValidationException()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "", PlayerNumber = 0, PlayerName = "" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() => service.AddPlayerToDepthChart(dto));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionRequired));
        }

        [Test]
        public async Task AddPlayerToDepthChart_ShiftsExistingEntries()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "Tom Brady", PositionDepth = 1 };
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var existingEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { DepthChartEntryId = 1, PlayerId = 2, PositionId = 1, DepthLevel = 0 },
                new DepthChartEntry { DepthChartEntryId = 2, PlayerId = 3, PositionId = 1, DepthLevel = 1 }
            };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetMaxDepthForPositionAsync(1)).ReturnsAsync(1);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(existingEntries);

            List<DepthChartEntry> updatedEntries = null;
            _mockDepthChartRepo.Setup(r => r.UpdateEntriesDepthAsync(It.IsAny<IEnumerable<DepthChartEntry>>()))
                .Callback<IEnumerable<DepthChartEntry>>(list => updatedEntries = list.ToList())
                .Returns(Task.CompletedTask);

            _mockDepthChartRepo.Setup(r => r.AddEntryAsync(It.IsAny<DepthChartEntry>())).ReturnsAsync(new DepthChartEntry());

            // Act
            var result = await service.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(updatedEntries, Is.Not.Null);
            Assert.That(updatedEntries.Count, Is.EqualTo(1));
            Assert.That(updatedEntries[0].DepthLevel, Is.EqualTo(2));
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_ShiftsRemainingEntries()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var removedEntry = new DepthChartEntry { DepthChartEntryId = 1, PlayerId = 1, PositionId = 1, DepthLevel = 1, Player = player };
            var remainingEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { DepthChartEntryId = 2, PlayerId = 2, PositionId = 1, DepthLevel = 2 },
                new DepthChartEntry { DepthChartEntryId = 3, PlayerId = 3, PositionId = 1, DepthLevel = 3 }
            };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.RemoveEntryAsync(1, 1)).ReturnsAsync(removedEntry);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(remainingEntries);

            List<DepthChartEntry> updatedEntries = null;
            _mockDepthChartRepo.Setup(r => r.UpdateEntriesDepthAsync(It.IsAny<IEnumerable<DepthChartEntry>>()))
                .Callback<IEnumerable<DepthChartEntry>>(list => updatedEntries = list.ToList())
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map<PlayerDto>(It.IsAny<Player>())).Returns(new PlayerDto());

            // Act
            var result = await service.RemovePlayerFromDepthChart("QB", 12);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(updatedEntries, Is.Not.Null);
            Assert.That(updatedEntries.Count, Is.EqualTo(2));
            Assert.That(updatedEntries[0].DepthLevel, Is.EqualTo(1));
            Assert.That(updatedEntries[1].DepthLevel, Is.EqualTo(2));
        }

        [Test]
        public async Task AddPlayerToDepthChart_WithoutPositionDepth_AddsToEnd()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "Tom Brady" };
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var existingEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { DepthChartEntryId = 1, PlayerId = 2, PositionId = 1, DepthLevel = 0 },
                new DepthChartEntry { DepthChartEntryId = 2, PlayerId = 3, PositionId = 1, DepthLevel = 1 }
            };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync((Player)null);
            _mockMapper.Setup(m => m.Map<Player>(It.IsAny<AddPlayerToDepthChartDto>())).Returns(player);
            _mockPlayerRepo.Setup(r => r.AddAsync(It.IsAny<Player>())).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetMaxDepthForPositionAsync(1)).ReturnsAsync(1);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(existingEntries);
            _mockDepthChartRepo.Setup(r => r.UpdateEntriesDepthAsync(It.IsAny<List<DepthChartEntry>>())).Returns(Task.CompletedTask);
            _mockDepthChartRepo.Setup(r => r.AddEntryAsync(It.IsAny<DepthChartEntry>())).ReturnsAsync(new DepthChartEntry());

            // Act
            var result = await service.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.True);
            _mockDepthChartRepo.Verify(r => r.AddEntryAsync(It.Is<DepthChartEntry>(e => e.DepthLevel == 2)), Times.Once);
        }

        [Test]
        public async Task AddPlayerToDepthChart_NewPosition_CreatesNewEntry()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var dto = new AddPlayerToDepthChartDto { Position = "K", PlayerNumber = 3, PlayerName = "John Smith" };
            var position = new Position { PositionId = 2, Name = "K" };
            var player = new Player { PlayerId = 4, Number = 3, Name = "John Smith" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("K")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(3)).ReturnsAsync((Player)null);
            _mockMapper.Setup(m => m.Map<Player>(It.IsAny<AddPlayerToDepthChartDto>())).Returns(player);
            _mockPlayerRepo.Setup(r => r.AddAsync(It.IsAny<Player>())).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetMaxDepthForPositionAsync(2)).ReturnsAsync(-1);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(2)).ReturnsAsync(new List<DepthChartEntry>());
            _mockDepthChartRepo.Setup(r => r.AddEntryAsync(It.IsAny<DepthChartEntry>())).ReturnsAsync(new DepthChartEntry());

            // Act
            var result = await service.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.True);
            _mockDepthChartRepo.Verify(r => r.AddEntryAsync(It.Is<DepthChartEntry>(e => e.DepthLevel == 0)), Times.Once);
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_LastPlayerInPosition_ReturnsPlayerAndLeavesEmptyPosition()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };
            var removedEntry = new DepthChartEntry { DepthChartEntryId = 1, PlayerId = 1, PositionId = 1, DepthLevel = 0, Player = player };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.RemoveEntryAsync(1, 1)).ReturnsAsync(removedEntry);
            _mockDepthChartRepo.Setup(r => r.GetEntriesByPositionAsync(1)).ReturnsAsync(new List<DepthChartEntry>());
            _mockMapper.Setup(m => m.Map<PlayerDto>(It.IsAny<Player>())).Returns(new PlayerDto { Number = 12, Name = "Tom Brady" });

            // Act
            var result = await service.RemovePlayerFromDepthChart("QB", 12);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Number, Is.EqualTo(12));
            Assert.That(result.Name, Is.EqualTo("Tom Brady"));
            _mockDepthChartRepo.Verify(r => r.UpdateEntriesDepthAsync(It.IsAny<List<DepthChartEntry>>()), Times.Never);
        }

        [Test]
        public async Task GetBackups_PlayerNotInSpecifiedPosition_ReturnsEmptyList()
        {
            // Arrange
            var service = _serviceProvider.GetService<IDepthChartService>();
            var position = new Position { PositionId = 1, Name = "QB" };
            var player = new Player { PlayerId = 1, Number = 12, Name = "Tom Brady" };

            _mockPositionRepo.Setup(r => r.GetByNameAsync("QB")).ReturnsAsync(position);
            _mockPlayerRepo.Setup(r => r.GetByNumberAsync(12)).ReturnsAsync(player);
            _mockDepthChartRepo.Setup(r => r.GetBackupsAsync(1, 1)).ReturnsAsync(new List<DepthChartEntry>());

            // Act
            var result = await service.GetBackups("QB", 12);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
