using System.ComponentModel.DataAnnotations;

namespace P2P.Domains.Entities;

public class Transactions
{
    [Key]
    public Guid TransactionId { get; private  set; }
    public decimal Amount { get; private set; }
    public CurrencyType Currency { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string? Description { get; private set; }
    public TransactionStatus? Status { get; private set; }
    public string? PaystackReference { get; private set; }
    public Guid? AccountId { get; private set; }

    public virtual Account Account { get; private set; }
    public Transactions(
        Guid accountId,
        decimal amount,
        CurrencyType currency,
        TransactionType transactionType,
        string? description = null,
        TransactionStatus? status = null,
        string? paystackReference = null)
    {
        TransactionId = Guid.NewGuid();
        AccountId = accountId;
        Amount = amount;
        Currency = currency;
        TransactionType = transactionType;
        Description = description;
        Status = status;
        PaystackReference = paystackReference;
        TransactionDate = DateTime.UtcNow;
    }

    public void UpdateStatus(TransactionStatus newStatus)
    {
     Status = newStatus;
    }
}