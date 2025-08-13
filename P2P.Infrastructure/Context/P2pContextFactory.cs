using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace P2P.Infrastructure.Context;

using dotenv.net;
public class P2pContextFactory: IDesignTimeDbContextFactory<P2pContext>
{
    public P2pContext CreateDbContext(string[] args)
    {
        DotEnv.Load();
        
        var optionsBuilder = new DbContextOptionsBuilder<P2pContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                               ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set");


        optionsBuilder.UseSqlServer(connectionString);
        
        return new P2pContext(optionsBuilder.Options);
   
    }
}