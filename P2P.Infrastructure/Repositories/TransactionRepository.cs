using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class TransactionRepository:ITransactionsRepository
{
    private readonly P2pContext _context;

    public TransactionRepository(P2pContext context)
    {
        _context = context;
    }


    public void Add(Transactions transaction)
    {
        if(transaction == null)
            throw new ArgumentNullException(nameof(transaction));
        
        _context.Add(transaction);
        _context.SaveChanges();
    }
    
    public void UpdateStatus(Guid transactionId, TransactionStatus newStatus)
    {
        var transaction = _context.Transactions.Find(transactionId);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with ID {transactionId} not found.");

        transaction.UpdateStatus(newStatus);
        _context.SaveChanges();
    }
}