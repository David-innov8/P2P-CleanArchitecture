using System.Linq.Expressions;

namespace P2P.Application.UseCases.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync<TKey>(TKey id);
    
    Task<IEnumerable<T>> GetAllAsync();
    
    Task AddAsync(T entity);

    void Update(T entity);
    void Delete(T entity);
    Task<T> Find(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate);

    IQueryable<T> Query();
}