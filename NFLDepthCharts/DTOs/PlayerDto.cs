namespace NFLDepthCharts.API.DTOs
{
    public class PlayerDto
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    public class PositionDto
    {
        public string Name { get; set; }
    }

    public class DepthChartEntryDto
    {
        public PlayerDto Player { get; set; }
        public PositionDto Position { get; set; }
        public int Depth { get; set; }
    }

    public class FullDepthChartDto
    {
        public Dictionary<string, List<PlayerDto>> Positions { get; set; }
    }
}
