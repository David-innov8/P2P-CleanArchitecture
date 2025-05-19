using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class AccountRepository: Repository<Account>,IAccountRepository
{
    

    public AccountRepository(P2pContext context):base(context) { }

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
        
   
    }
