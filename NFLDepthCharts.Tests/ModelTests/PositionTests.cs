using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.Tests.ModelTests
{
    [TestFixture]
    public class PositionTests
    {
        [Test]
        public void Position_Properties_SetAndRetrieveCorrectly()
        {
            // Arrange
            var position = new Position
            {
                PositionId = 1,
                Name = "Quarterback"
            };

            // Act & Assert
            Assert.That(position.PositionId, Is.EqualTo(1));
            Assert.That(position.Name, Is.EqualTo("Quarterback"));
        }
    }
}
