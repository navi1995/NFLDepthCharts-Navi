using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Data
{
    public interface IDepthChartDbContext
    {
        DbSet<Player> Players { get; set; }
        DbSet<Position> Positions { get; set; }
        DbSet<DepthChartEntry> DepthChartEntries { get; set; }

        Task<int> SaveChangesAsync();
        void Dispose();
        int SaveChanges();
    }
}
