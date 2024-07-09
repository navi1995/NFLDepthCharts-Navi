using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Validators;

namespace NFLDepthCharts.Tests.ValidatorTests
{
    [TestFixture]
    public class DepthChartEntryValidatorTests
    {
        private IDepthChartEntryValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new DepthChartEntryValidator();
        }

        [Test]
        public void Validate_ValidEntry_DoesNotThrowException()
        {
            var entry = new DepthChartEntry { PlayerId = 1, PositionId = 1, DepthLevel = 0 };
            Assert.DoesNotThrow(() => _validator.Validate(entry));
        }

        [Test]
        public void Validate_NullEntry_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _validator.Validate(null));
        }

        [Test]
        public void Validate_NegativeDepthLevel_ThrowsValidationException()
        {
            var entry = new DepthChartEntry { PlayerId = 1, PositionId = 1, DepthLevel = -1 };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(entry));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.DepthLevelNegative));
        }

        [Test]
        public void Validate_InvalidPlayerId_ThrowsValidationException()
        {
            var entry = new DepthChartEntry { PlayerId = 0, PositionId = 1, DepthLevel = 0 };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(entry));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PlayerIdInvalid));
        }

        [Test]
        public void Validate_InvalidPositionId_ThrowsValidationException()
        {
            var entry = new DepthChartEntry { PlayerId = 1, PositionId = 0, DepthLevel = 0 };
            var ex = Assert.Throws<ValidationException>(() => _validator.Validate(entry));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PositionIdInvalid));
        }
    }
}
