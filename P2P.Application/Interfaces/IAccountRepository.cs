using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces;

public interface IAccountRepository
{
    Task<Account> GetAccountByNumberAsync(string accountNumber);
    Task AddAccountAsync(Account account);
    Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId);
    Task<bool> AccountNumberExistsAsync(string accountNumber);
}