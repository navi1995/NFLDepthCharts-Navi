using NFLDepthCharts.API.DTOs;

namespace NFLDepthCharts.API.Services
{
    public interface IDepthChartService
    {
        Task<bool> AddPlayerToDepthChart(AddPlayerToDepthChartDto dto);
        Task<IEnumerable<PlayerDto>> GetBackups(string position, int playerNumber);
        Task<FullDepthChartDto> GetFullDepthChart();
        Task<PlayerDto> RemovePlayerFromDepthChart(string position, int playerNumber);
    }
}
