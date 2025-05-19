using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class TransactionRepository: Repository<Transactions>,ITransactionsRepository
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly P2pContext _context;
 
   
    public TransactionRepository(P2pContext context, IUnitOfWork unitOfWork):base(context)
    {
        _unitOfWork = unitOfWork;
 
    }

    public async Task AddAsync(Transactions transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

       await base.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
    }


    public async Task UpdateStatusAsync(Guid transactionId, TransactionStatus newStatus)
    {
        var transaction = await base.Find(u => u.TransactionId == transactionId);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with ID {transactionId} not found.");

        transaction.UpdateStatus(newStatus);
        base.Update(transaction);
        await _unitOfWork.SaveChangesAsync();
    }
   
    public async Task AddRangeAsync(IEnumerable<Transactions> transactions)
    {
        if (transactions == null || !transactions.Any())
            throw new ArgumentException("Transactions cannot be null or empty", nameof(transactions));

        foreach (var transaction in transactions)
        {
            base.AddAsync(transaction);
        }

        await _unitOfWork.SaveChangesAsync();
    }
    

    public async Task<IEnumerable<Transactions>> GetByAccountIdAsync(Guid accountId)
    {
        return await base.FindAll(t => t.AccountId == accountId);
    }

    public async Task<IEnumerable<Transactions>> GetTransactionsByUserIdAsync(Guid userId, int pageNumber, int pageSize)
    {
        var transactions = await _unitOfWork
            .GetRepository<Transactions>()
            .Query()
            .Include(t => t.Account)
            .Where(t => t.Account.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return transactions;
    }
 
    
    public async Task<int> CountTransactionsByUserIdAsync(Guid userId)
    {
        return await _unitOfWork.GetRepository<Transactions>()
            .Query()
            .Include(t => t.Account)
            .Where(t => t.Account.UserId == userId)
            .CountAsync();
    }
}