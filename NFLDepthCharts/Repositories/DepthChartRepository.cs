using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Repositories
{

    public class DepthChartRepository : IDepthChartRepository
    {
        private readonly IDepthChartDbContext _context;

        public DepthChartRepository(IDepthChartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepthChartEntry>> GetEntriesByPositionAsync(int positionId)
        {
            return await _context.DepthChartEntries
                .Where(d => d.PositionId == positionId)
                .OrderBy(d => d.DepthLevel)
                .ToListAsync();
        }

        public async Task<int> GetMaxDepthForPositionAsync(int positionId)
        {
            return await _context.DepthChartEntries
                .Where(d => d.PositionId == positionId)
                .MaxAsync(d => (int?)d.DepthLevel) ?? -1;
        }

        public async Task UpdateEntriesDepthAsync(IEnumerable<DepthChartEntry> entries)
        {
            _context.DepthChartEntries.UpdateRange(entries);
            await _context.SaveChangesAsync();
        }

        public async Task AddEntryAsync(DepthChartEntry entry)
        {
            _context.DepthChartEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<DepthChartEntry> RemoveEntryAsync(int positionId, int playerId)
        {
            var entry = await _context.DepthChartEntries
                .FirstOrDefaultAsync(d => d.PositionId == positionId && d.PlayerId == playerId);

            if (entry != null)
            {
                _context.DepthChartEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }

            return entry;
        }

        public async Task<IEnumerable<DepthChartEntry>> GetBackupsAsync(int positionId, int playerId)
        {
            var playerDepth = await _context.DepthChartEntries
                .Where(d => d.PositionId == positionId && d.PlayerId == playerId)
                .Select(d => d.DepthLevel)
                .FirstOrDefaultAsync();

            return await _context.DepthChartEntries
                .Where(d => d.PositionId == positionId && d.DepthLevel > playerDepth)
                .OrderBy(d => d.DepthLevel)
                .Include(d => d.Player) // Needed for output DTO
                .ToListAsync();
        }

        public async Task<IDictionary<string, IEnumerable<Player>>> GetFullDepthChartAsync()
        {
            var entries = await _context.DepthChartEntries
                .Include(d => d.Position)
                .Include(d => d.Player)
                .OrderBy(d => d.Position.Name)
                .ThenBy(d => d.DepthLevel)
                .ToListAsync();

            return entries
                .GroupBy(d => d.Position.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(d => d.Player)
                );
        }
    }
}