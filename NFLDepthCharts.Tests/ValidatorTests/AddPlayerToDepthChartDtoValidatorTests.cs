using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Validators;

namespace NFLDepthCharts.Tests.ValidatorTests
{
    [TestFixture]
    public class AddPlayerToDepthChartDtoValidatorTests
    {
        private IAddPlayerToDepthChartDtoValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new AddPlayerToDepthChartDtoValidator();
        }

        [Test]
        public void Validate_ValidDto_DoesNotThrowException()
        {
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "Tom Brady" };
            Assert.DoesNotThrow(() => _validator.Validate(dto));
        }

        [Test]
        public void Validate_NullDto_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _validator.Validate(null));
        }

        [Test]
        public void Validate_EmptyPosition_ThrowsValidationException()
        {
            var dto = new AddPlayerToDepthChartDto { Position = "", PlayerNumber = 12, PlayerName = "Tom Brady" };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(dto));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionRequired));
        }

        [Test]
        public void Validate_InvalidPlayerNumber_ThrowsValidationException()
        {
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 0, PlayerName = "Tom Brady" };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(dto));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNumberInvalid));
        }

        [Test]
        public void Validate_EmptyPlayerName_ThrowsValidationException()
        {
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "" };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(dto));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNameRequired));
        }

        [Test]
        public void Validate_NegativePositionDepth_ThrowsValidationException()
        {
            var dto = new AddPlayerToDepthChartDto { Position = "QB", PlayerNumber = 12, PlayerName = "Tom Brady", PositionDepth = -1 };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(dto));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionDepthNegative));
        }
    }
}
