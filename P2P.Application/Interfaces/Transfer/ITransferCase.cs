using P2P.Domains;

namespace P2P.Application.UseCases.Interfaces.Transfer;

public interface ITransferCase
{
    Task ExecuteTransfer(string recipientUsername, decimal amount, CurrencyType currency);
}