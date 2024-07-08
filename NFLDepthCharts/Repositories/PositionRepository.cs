using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{

    public class PositionRepository : IPositionRepository
    {
        private readonly IDepthChartDbContext _context;

        public PositionRepository(IDepthChartDbContext context)
        {
            _context = context;
        }

        public async Task<Position> GetByNameAsync(string name)
        {
            return await _context.Positions.FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}
