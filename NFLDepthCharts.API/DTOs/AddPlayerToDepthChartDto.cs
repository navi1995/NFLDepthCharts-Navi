using System.ComponentModel.DataAnnotations;

namespace NFLDepthCharts.API.DTOs
{
    public class AddPlayerToDepthChartDto
    {
        public string Position { get; set; }
        public int PlayerNumber { get; set; }
        public string PlayerName { get; set; }
        public int? PositionDepth { get; set; }
    }
}
