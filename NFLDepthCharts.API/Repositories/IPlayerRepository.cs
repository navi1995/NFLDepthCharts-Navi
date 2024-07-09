using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetByNumberAsync(int number);
        Task<Player> AddAsync(Player player);
    }
}
