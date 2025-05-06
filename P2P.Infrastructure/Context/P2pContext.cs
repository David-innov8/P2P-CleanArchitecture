using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P2P.Domains.Entities;
using P2P.Domains.ValueObjects;

namespace P2P.Infrastructure.Context;

public class P2pContext :DbContext
{
    public P2pContext(DbContextOptions<P2pContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<GeneralLedger> GeneralLedgers { get; set; }
    public DbSet<GlTransactions> GlTransactions { get; set; }
    
    public DbSet<Transactions> Transactions { get; set; }

    public class GeneralLedgerConfiguration : IEntityTypeConfiguration<GeneralLedger>
    {
        public void Configure(EntityTypeBuilder<GeneralLedger> builder)
        {
            builder.HasKey(gl => gl.Id);
        
            // Configure GLAccountNumber as a value object
            builder.Property(gl => gl.AccountNumber)
                .HasConversion(
                    v => v.ToString(),
                    v => new GLAccountNumber(v));

            // Other configuration...
            builder.Property(gl => gl.Balance).HasPrecision(18, 2);
            builder.Property(gl => gl.MinimumBalance).HasPrecision(18, 2);
        }
        
        
    }

   
        
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
    
        modelBuilder.Entity<Transactions>()
            .HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        modelBuilder.ApplyConfiguration(new GeneralLedgerConfiguration());
        
        modelBuilder.Entity<GlTransactions>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.GlId).IsRequired();
            entity.Property(t => t.UserId).IsRequired();
            entity.Property(t => t.TransactionId).IsRequired();

            // Relationships
            entity.HasOne<GeneralLedger>()
                .WithMany()
                .HasForeignKey(t => t.GlId);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId);
        });
    }
        
    
}