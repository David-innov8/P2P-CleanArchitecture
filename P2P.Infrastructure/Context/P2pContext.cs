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
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Username).IsRequired();
            builder.Property(e => e.Email).IsRequired();
            builder.HasIndex(e => e.Email).IsUnique();
            builder.HasIndex(e => e.Username).IsUnique();
            
            builder.HasMany(e => e.Accounts)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(u => u.Password);
            builder.OwnsOne(u => u.Pin);
            builder.OwnsOne(u => u.Profile);
            builder.OwnsOne(u => u.State);
            builder.OwnsOne(u => u.Audit);
            builder.OwnsOne(u => u.EngagementMetrics);
            builder.OwnsOne(u => u.Consent);
        });
        
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountNumber).IsRequired();
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).IsRequired();
        });
    }
        
    
}