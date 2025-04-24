namespace P2P.Domains.Entities;

public class GlTransactions
{
    public Guid Id { get; private set; } // Use GUID for better uniqueness
    public decimal Amount { get; private set; }
    public CurrencyType Currency { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public TransactionStatus? Status { get; private set; }
    public decimal OldBalance { get; private set; }
    public decimal NewBalance { get; private set; }
    public string? PaystackReference { get; private set; }

    // Foreign keys
    public Guid GlId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid TransactionId { get; private set; }
    
    public GlTransactions(
        Guid glId,
        Guid userId,
        Guid transactionId,
        decimal amount,
        CurrencyType currency,
        TransactionType transactionType,
        decimal oldBalance,
        decimal newBalance,
        TransactionStatus? status = null,
        string? paystackReference = null)
    {
        Id = Guid.NewGuid();
        GlId = glId;
        UserId = userId;
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        TransactionType = transactionType;
        OldBalance = oldBalance;
        NewBalance = newBalance;
        Status = status;
        PaystackReference = paystackReference;
        TransactionDate = DateTime.UtcNow;
    }
}