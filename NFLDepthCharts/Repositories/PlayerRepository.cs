using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{

    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDepthChartDbContext _context;

        public PlayerRepository(IDepthChartDbContext context)
        {
            _context = context;
        }

        public async Task<Player> GetByNumberAsync(int number)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.Number == number);
        }

        public async Task<Player> AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }
    }
}
