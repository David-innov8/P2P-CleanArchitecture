namespace P2P.Application.UseCases.Interfaces;

public interface IUnitOfWork: IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
  

    void Dispose();
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();

    Task SaveChangesAsync();
}