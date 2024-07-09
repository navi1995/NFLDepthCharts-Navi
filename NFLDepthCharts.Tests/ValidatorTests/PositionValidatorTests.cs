using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Validators;

namespace NFLDepthCharts.Tests.ValidatorTests
{
    [TestFixture]
    public class PositionValidatorTests
    {
        private IPositionValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new PositionValidator();
        }

        [Test]
        public void Validate_ValidPosition_DoesNotThrowException()
        {
            var position = new Position { Name = "QB" };
            Assert.DoesNotThrow(() => _validator.Validate(position));
        }

        [Test]
        public void Validate_NullPosition_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _validator.Validate(null));
        }

        [Test]
        public void Validate_EmptyName_ThrowsValidationException()
        {
            var position = new Position { Name = "" };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(position));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionNameRequired));
        }

        [Test]
        public void Validate_NameTooLong_ThrowsValidationException()
        {
            var position = new Position { Name = new string('A', 51) };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(position));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionNameTooLong));
        }
    }
}
