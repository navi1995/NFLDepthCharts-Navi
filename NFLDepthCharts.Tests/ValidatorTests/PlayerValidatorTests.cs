using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Validators;

namespace NFLDepthCharts.Tests.ValidatorTests
{
    [TestFixture]
    public class PlayerValidatorTests
    {
        private IPlayerValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new PlayerValidator();
        }

        [Test]
        public void Validate_ValidPlayer_DoesNotThrowException()
        {
            var player = new Player { Number = 12, Name = "Tom Brady" };
            Assert.DoesNotThrow(() => _validator.Validate(player));
        }

        [Test]
        public void Validate_NullPlayer_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _validator.Validate(null));
        }

        [Test]
        public void Validate_EmptyName_ThrowsValidationException()
        {
            var player = new Player { Number = 12, Name = "" };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(player));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNameRequired));
        }

        [Test]
        public void Validate_InvalidNumber_ThrowsValidationException()
        {
            var player = new Player { Number = 0, Name = "Tom Brady" };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(player));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNumberInvalid));
        }

        [Test]
        public void Validate_InvalidNameLength_ThrowsValidationException()
        {
            var player = new Player { Number = 0, Name = new string('a', 101) };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(player));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerNameTooLong));
        }
    }
}
