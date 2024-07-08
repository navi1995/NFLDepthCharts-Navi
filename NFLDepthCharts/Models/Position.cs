namespace NFLDepthCharts.API.Models
{
    public class Position : IPosition
    {
        public int PositionId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DepthChartEntry> PositionPlayerDepthEntries { get; set; }
        // public string DisplayName { get; set; } e.g Quarter Back
        // public PositionType { get; set; } e.g Offense, Defense etc, could be an Enum
    }
}
