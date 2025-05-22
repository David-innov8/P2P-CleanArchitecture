using P2P.Domains;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.Transfer;

public interface ITransactionsRepository : IRepository<Transactions>
{
    Task AddAsync(Transactions transaction);
    Task UpdateStatusAsync(Guid transactionId, TransactionStatus newStatus);
    Task AddRangeAsync(IEnumerable<Transactions> transactions);
    Task<IEnumerable<Transactions>> GetByAccountIdAsync(Guid accountId);
    
    Task<int> CountTransactionsByUserIdAsync(Guid userId);
    Task<IEnumerable<Transactions>> GetTransactionsByUserIdAsync(Guid userId, int pageNumber, int pageSize);

    IQueryable<Transactions> Query();

    IQueryable<Transactions> QueryWithIncludes(params string[] includes);

}