
namespace NFLDepthCharts.API.Models
{
    public interface IPlayer
    {
        string Name { get; set; }
        int Number { get; set; }
        int PlayerId { get; set; }
        ICollection<DepthChartEntry> PositionPlayerDepthEntries { get; set; }
    }
}