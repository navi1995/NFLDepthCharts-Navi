using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.Tests.ModelTests
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Player_Properties_SetAndRetrieveCorrectly()
        {
            // Arrange
            var player = new Player
            {
                PlayerId = 1,
                Number = 12,
                Name = "Tom Brady"
            };

            // Act & Assert
            Assert.That(player.PlayerId, Is.EqualTo(1));
            Assert.That(player.Number, Is.EqualTo(12));
            Assert.That(player.Name, Is.EqualTo("Tom Brady"));
        }
    }
}
