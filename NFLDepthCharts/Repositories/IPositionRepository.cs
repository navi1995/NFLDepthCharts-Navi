using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{
    public interface IPositionRepository
    {
        Task<Position> GetByNameAsync(string name);
    }
}
