namespace NFLDepthCharts.API.Models
{
    public class DepthChartEntry : IDepthChartEntry
    {
        public int DepthChartEntryId { get; set; }
        public int DepthId { get; set; }
        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
        public int DepthLevel { get; set; }
    }
}
