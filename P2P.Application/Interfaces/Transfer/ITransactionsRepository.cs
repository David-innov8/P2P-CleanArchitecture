using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.Transfer;

public interface ITransactionsRepository
{
    void Add(Transactions transaction);
}