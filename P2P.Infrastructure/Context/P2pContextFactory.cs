using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace P2P.Infrastructure.Context;

public class P2pContextFactory: IDesignTimeDbContextFactory<P2pContext>
{
    public P2pContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<P2pContext>();
        
        // Use the same connection string from appsettings.json
        var connectionString = "Server=localhost;Database=P2PwalletClean;User Id=SA;Password=MyStrongPass123;TrustServerCertificate=true;";
        
        optionsBuilder.UseSqlServer(connectionString);
        
        return new P2pContext(optionsBuilder.Options);
   
    }
}