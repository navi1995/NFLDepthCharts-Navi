using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Data
{
    public class DepthChartDbContext : DbContext, IDepthChartDbContext
    {
        public DepthChartDbContext(DbContextOptions<DepthChartDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<DepthChartEntry> DepthChartEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DepthChartEntry>()
                .HasKey(d => d.DepthChartEntryId);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.PositionPlayerDepthEntries)
                .WithOne(d => d.Player)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Position>()
                .HasMany(p => p.PositionPlayerDepthEntries)
                .WithOne(d => d.Position)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DepthChartEntry>()
                .HasOne(d => d.Player)
                .WithMany(p => p.PositionPlayerDepthEntries)
                .HasForeignKey(d => d.PlayerId);

            modelBuilder.Entity<DepthChartEntry>()
                .HasOne(d => d.Position)
                .WithMany(p => p.PositionPlayerDepthEntries)
                .HasForeignKey(d => d.PositionId);

            modelBuilder.Entity<Player>()
                .HasIndex(p => p.Number);

            modelBuilder.Entity<Position>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<DepthChartEntry>()
                .HasIndex(d => new { d.PositionId, d.DepthLevel });

            modelBuilder.Entity<DepthChartEntry>()
                .HasIndex(d => d.PlayerId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
