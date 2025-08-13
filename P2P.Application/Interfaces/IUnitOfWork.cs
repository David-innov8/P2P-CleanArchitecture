namespace P2P.Application.UseCases.Interfaces;

public interface IUnitOfWork: IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

    Task<T> ExecuteWithinStrategy<T>(Func<Task<T>> operation);
    Task ExecuteInTransactionAsync(Func<Task> operation);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void Dispose();
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();

    Task SaveChangesAsync();
    
    // Task<T> ExecuteSqlScalarAsync<T>(string sql, params object[] parameters);
}