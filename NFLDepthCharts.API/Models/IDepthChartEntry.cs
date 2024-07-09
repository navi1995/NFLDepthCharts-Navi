namespace NFLDepthCharts.API.Models
{
    public interface IDepthChartEntry
    {
        int DepthChartEntryId { get; set; }
        int DepthId { get; set; }
        int DepthLevel { get; set; }
        Player Player { get; set; }
        int PlayerId { get; set; }
        Position Position { get; set; }
        int PositionId { get; set; }
    }
}