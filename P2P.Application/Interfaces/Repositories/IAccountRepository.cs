using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces;

public interface IAccountRepository:IRepository<Account>
{
    Task<Account> GetAccountByNumberAsync(string accountNumber);

    Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId);
    Task<bool> AccountNumberExistsAsync(string accountNumber);
    
  
}