namespace NFLDepthCharts.API.Models
{
    public class Player : IPlayer
    {
        public int PlayerId { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DepthChartEntry> PositionPlayerDepthEntries { get; set; }
    }
}
