namespace NFLDepthCharts.API.Models
{
    // Unused class, to show future expandability. DepthChartEntry could have a TeamId, which would also provide it with a sport. 
    public class Team
    {
        public int TeamId { get; set; }
        public int SportId { get; set; }
        public Sport Sport { get; set; }
    }
}
