using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{

    public class PositionRepository : IPositionRepository
    {
        private readonly IDepthChartDbContext _context;
        private readonly ILogger<IPositionRepository> _logger;

        public PositionRepository(IDepthChartDbContext context, ILogger<IPositionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Position> GetByNameAsync(string name)
        {
            _logger.LogInformation("Getting Position by name");

            return await _context
                .Positions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}
