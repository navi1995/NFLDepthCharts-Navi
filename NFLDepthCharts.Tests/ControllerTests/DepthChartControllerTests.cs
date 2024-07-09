using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NFLDepthCharts.API.Controllers;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Services;

namespace NFLDepthCharts.Tests.ControllerTests
{
    [TestFixture]
    public class DepthChartControllerTests
    {
        private Mock<IDepthChartService> _mockService;
        private Mock<ILogger<DepthChartController>> _loggerMock;
        private DepthChartController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IDepthChartService>();
            _loggerMock = new Mock<ILogger<DepthChartController>>();
            _controller = new DepthChartController(_mockService.Object, _loggerMock.Object);
        }

        [Test]
        public async Task AddPlayerToDepthChart_ReturnsCreatedAtAction_WhenServiceReturnsTrue()
        {
            // Arrange
            var dto = TestDataFactory.CreateAddPlayerToDepthChartDto();
            _mockService.Setup(s => s.AddPlayerToDepthChart(It.IsAny<AddPlayerToDepthChartDto>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(DepthChartController.AddPlayerToDepthChart)));
        }

        [Test]
        public async Task AddPlayerToDepthChart_ReturnsBadRequest_WhenServiceReturnsFalse()
        {
            // Arrange
            var dto = TestDataFactory.CreateAddPlayerToDepthChartDto();
            _mockService.Setup(s => s.AddPlayerToDepthChart(It.IsAny<AddPlayerToDepthChartDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddPlayerToDepthChart(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void AddPlayerToDepthChart_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            var dto = TestDataFactory.CreateAddPlayerToDepthChartDto();
            _mockService.Setup(s => s.AddPlayerToDepthChart(It.IsAny<AddPlayerToDepthChartDto>()))
                .ThrowsAsync(new ValidationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.AddPlayerToDepthChart(dto));
            // This will be caught in ApiExceptionMiddleware and returned there 
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_ReturnsOkWithPlayer_WhenPlayerRemoved()
        {
            // Arrange
            var playerDto = TestDataFactory.CreatePlayerDto();
            _mockService.Setup(s => s.RemovePlayerFromDepthChart(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(playerDto);

            // Act
            var actionResult = await _controller.RemovePlayerFromDepthChart("QB", 12);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(playerDto));
        }

        [Test]
        public async Task RemovePlayerFromDepthChart_ReturnsNotFound_WhenPlayerNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.RemovePlayerFromDepthChart(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((PlayerDto)null);

            // Act
            var actionResult = await _controller.RemovePlayerFromDepthChart("QB", 12);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void RemovePlayerFromDepthChart_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            _mockService.Setup(s => s.RemovePlayerFromDepthChart(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new ValidationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.RemovePlayerFromDepthChart("QB", 12));
            // This will be caught in ApiExceptionMiddleware and returned there 
        }

        [Test]
        public async Task GetBackups_ReturnsOkWithBackups_WhenBackupsFound()
        {
            // Arrange
            var backups = new List<PlayerDto> { TestDataFactory.CreatePlayerDto(), TestDataFactory.CreatePlayerDto() };
            _mockService.Setup(s => s.GetBackups(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(backups);

            // Act
            var actionResult = await _controller.GetBackups("QB", 12);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(backups));
        }

        [Test]
        public async Task GetBackups_ReturnsOkWithEmptyList_WhenNoBackupsFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetBackups(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new List<PlayerDto>());

            // Act
            var actionResult = await _controller.GetBackups("QB", 12);

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That((okResult.Value as IEnumerable<PlayerDto>), Is.Empty);
        }

        [Test]
        public void GetBackups_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            _mockService.Setup(s => s.GetBackups(It.IsAny<string>(), It.IsAny<int>()))
                .ThrowsAsync(new ValidationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetBackups("QB", 12));
            // This will be caught in ApiExceptionMiddleware and returned there 
        }

        [Test]
        public async Task GetFullDepthChart_ReturnsOkWithDepthChart_WhenDepthChartFound()
        {
            // Arrange
            var depthChart = TestDataFactory.CreateFullDepthChartDto();
            _mockService.Setup(s => s.GetFullDepthChart())
                .ReturnsAsync(depthChart);

            // Act
            var actionResult = await _controller.GetFullDepthChart();

            // Assert
            Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(depthChart));
        }

        [Test]
        public void GetFullDepthChart_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            _mockService.Setup(s => s.GetFullDepthChart())
                .ThrowsAsync(new ValidationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _controller.GetFullDepthChart());
            // This will be caught in ApiExceptionMiddleware and returned there 
        }
    }

    public static class TestDataFactory
    {
        public static AddPlayerToDepthChartDto CreateAddPlayerToDepthChartDto()
        {
            return new AddPlayerToDepthChartDto
            {
                Position = "QB",
                PlayerNumber = 12,
                PlayerName = "Tom Brady",
                PositionDepth = 1
            };
        }

        public static PlayerDto CreatePlayerDto()
        {
            return new PlayerDto
            {
                Number = 12,
                Name = "Tom Brady"
            };
        }

        public static FullDepthChartDto CreateFullDepthChartDto()
        {
            return new FullDepthChartDto
            {
                Positions = new Dictionary<string, List<PlayerDto>>
            {
                { "QB", new List<PlayerDto> { CreatePlayerDto() } }
            }
            };
        }
    }
}
