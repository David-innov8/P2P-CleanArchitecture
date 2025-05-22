using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    public readonly P2pContext _context;
    private readonly DbSet<T> _entities;

    public Repository(P2pContext context)
    {
        _context = context;
        _entities = _context.Set<T>();
    }

    public async Task  AddAsync(T entity)
    {
        await _entities.AddAsync(entity);
    }
 
    
    public async   Task<T> GetByIdAsync<TKey>(TKey id)
    {
     return await  _entities.FindAsync(new object[] { id });

    }

    public void Update(T entity)
    {
         _entities.Update(entity);
    }
    
    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        await Task.Run(() => _entities.UpdateRange(entities));
    }

    public void Delete(T entity)
    {
        _entities.Remove(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _entities.ToListAsync();
    }
    
    public async Task<T> Find(Expression<Func<T, bool >> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
    
    public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Where(predicate).ToListAsync();
    }
    
    public IQueryable<T> Query()
    {
        return _entities.AsQueryable();
    }


}