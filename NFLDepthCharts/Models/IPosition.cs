
namespace NFLDepthCharts.API.Models
{
    public interface IPosition
    {
        string Name { get; set; }
        int PositionId { get; set; }
        ICollection<DepthChartEntry> PositionPlayerDepthEntries { get; set; }
    }
}