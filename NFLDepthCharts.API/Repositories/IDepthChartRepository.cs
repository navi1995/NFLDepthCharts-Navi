using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{
    public interface IDepthChartRepository
    {
        Task<IEnumerable<DepthChartEntry>> GetEntriesByPositionAsync(int positionId);
        Task<int> GetMaxDepthForPositionAsync(int positionId);
        Task UpdateEntriesDepthAsync(IEnumerable<DepthChartEntry> entries);
        Task<DepthChartEntry> AddEntryAsync(DepthChartEntry entry);
        Task<DepthChartEntry> RemoveEntryAsync(int positionId, int playerId);
        Task<IEnumerable<DepthChartEntry>> GetBackupsAsync(int positionId, int playerId);
        Task<IDictionary<string, IEnumerable<Player>>> GetFullDepthChartAsync();
    }
}