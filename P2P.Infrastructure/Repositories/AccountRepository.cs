using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class AccountRepository: Repository<Account>,IAccountRepository
{
    private readonly IUnitOfWork _unitOfWork;
 

    public AccountRepository(P2pContext context, IUnitOfWork unitOfWork) : base(context)
    {
        
        _unitOfWork = unitOfWork;   
     
    }

      public async Task<Account> GetAccountByNumberAsync(string accountNumber)
        {
            return await Find(u=>u.AccountNumber == accountNumber);
        }

       

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(Guid userId)
        {
            return await FindAll(u=> (u.UserId) == userId);
        }

      
        public async Task<bool> AccountNumberExistsAsync(string accountNumber)
        {
      
            return await Find(a=> a.AccountNumber == accountNumber) != null;


        }

        public async Task<string> GetHighestAccountNumberAsync()
        {
           
            var highestAccount = await _context.Accounts
                .OrderByDescending(a => a.AccountNumber)
                .Select(a => a.AccountNumber)
                .FirstOrDefaultAsync();
    
            return highestAccount ?? string.Empty;
        }
        
        public async Task<Dictionary<string, bool>> CheckAccountNumbersExistAsync(List<string> accountNumbers)
        {
            // This efficiently checks multiple account numbers in one database query
            var result = new Dictionary<string, bool>();
    
            if (accountNumbers == null || accountNumbers.Count == 0)
            {
                return result;
            }
    
            // Find which account numbers already exist in the database
            var existingNumbers = await _context.Accounts
                .Where(a => accountNumbers.Contains(a.AccountNumber))
                .Select(a => a.AccountNumber)
                .ToListAsync();
    
            // Create a result dictionary
            foreach (var number in accountNumbers)
            {
                result[number] = existingNumbers.Contains(number);
            }
    
            return result;
        }
        
        // public async Task UpdateAccountBalanceAsync(Guid accountId,decimal amount)
        // {
        //     var account = await base.Find(a => a.Id == accountId);
        //     account.Balance += amount;
        // }
        
     
   
    }
