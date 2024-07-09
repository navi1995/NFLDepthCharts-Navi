using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.Tests.ModelTests
{
    [TestFixture]
    public class DepthChartEntryTests
    {
        [Test]
        public void DepthChartEntry_Properties_SetAndRetrieveCorrectly()
        {
            // Arrange
            var entry = new DepthChartEntry
            {
                DepthChartEntryId = 1,
                PlayerId = 2,
                PositionId = 3,
                DepthLevel = 1
            };

            // Act & Assert
            Assert.That(entry.DepthChartEntryId, Is.EqualTo(1));
            Assert.That(entry.PlayerId, Is.EqualTo(2));
            Assert.That(entry.PositionId, Is.EqualTo(3));
            Assert.That(entry.DepthLevel, Is.EqualTo(1));
        }

        [Test]
        public void DepthChartEntry_NavigationProperties_InitializedAsNull()
        {
            // Arrange
            var entry = new DepthChartEntry();

            // Act & Assert
            Assert.That(entry.Player, Is.Null);
            Assert.That(entry.Position, Is.Null);
        }
    }
}
