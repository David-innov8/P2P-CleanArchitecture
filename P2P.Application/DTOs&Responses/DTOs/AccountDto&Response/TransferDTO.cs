using P2P.Domains;

namespace P2P.Application.DTOs.AccountDto_Response;

public class TransferDTO
{
    public Guid TransactionId{get;set;}
    public decimal Amount{get;set;}
    public CurrencyType Currency{get;set;}
    public TransactionStatus Status{get;set;}
}