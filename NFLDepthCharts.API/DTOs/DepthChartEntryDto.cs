namespace NFLDepthCharts.API.DTOs
{
    public class DepthChartEntryDto
    {
        public PlayerDto Player { get; set; }
        public PositionDto Position { get; set; }
        public int Depth { get; set; }
    }
}
