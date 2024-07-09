using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{

    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDepthChartDbContext _context;
        private readonly ILogger<IPlayerRepository> _logger;

        public PlayerRepository(IDepthChartDbContext context, ILogger<IPlayerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Player> GetByNumberAsync(int number)
        {
            _logger.LogInformation("Getting Player by Number");

            return await _context
                .Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Number == number);
        }

        public async Task<Player> AddAsync(Player player)
        {
            _logger.LogInformation("Adding Player");

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }
    }
}
