using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.GeneralLedgers;

public interface IGLTransactionRepository:IRepository<GlTransactions>
{
    Task<GlTransactions> GetByReferenceAsync(string reference);
}