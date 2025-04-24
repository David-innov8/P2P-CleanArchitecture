using Microsoft.EntityFrameworkCore;
using P2P.Domains.Entities;

namespace P2P.Infrastructure.Context;

public class P2pContext :DbContext
{
    public P2pContext(DbContextOptions<P2pContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }



   
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(builder =>
        {
            builder.OwnsOne(u => u.Password);
            builder.OwnsOne(u => u.Pin);
            builder.OwnsOne(u => u.Profile);
            builder.OwnsOne(u => u.State);
            builder.OwnsOne(u => u.Audit);
            builder.OwnsOne(u => u.EngagementMetrics);
            builder.OwnsOne(u => u.Consent);
        });
    }
        
    
}