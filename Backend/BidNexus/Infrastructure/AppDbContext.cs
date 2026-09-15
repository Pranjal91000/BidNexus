using Microsoft.EntityFrameworkCore;
using Core.Entities.Auction;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuctionRequirements> Auctions => Set<AuctionRequirements>();

    }
}       
