using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class AccountRepository: IAccountRepository
{
    
    private readonly P2pContext _dbContext;

    public AccountRepository(P2pContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

      public async Task<Account> GetAccountByNumberAsync(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty", nameof(accountNumber));

            return await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task AddAccountAsync(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _dbContext.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> AccountNumberExistsAsync(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number cannot be null or empty", nameof(accountNumber));

            return await _dbContext.Accounts
                .AnyAsync(a => a.AccountNumber == accountNumber);
        }
    }
