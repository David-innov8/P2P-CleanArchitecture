using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using P2P.Application.UseCases.Interfaces;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class UnitOfWork: IUnitOfWork
{
    private readonly P2pContext _context;
    private Dictionary<Type, object> _repositories;
    private IDbContextTransaction _transaction;

    public UnitOfWork(P2pContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
        
    }
    
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        if (_repositories.ContainsKey(typeof(TEntity)))
        {
            return (IRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        var repository = new Repository<TEntity>(_context);
        _repositories.Add(typeof(TEntity), repository);
        return repository;
    }
    

    public void Dispose()
    {
        _context.Dispose();
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

   
}