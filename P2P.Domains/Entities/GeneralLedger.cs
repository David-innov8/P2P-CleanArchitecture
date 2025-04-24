using P2P.Domains.ValueObjects;

namespace P2P.Domains.Entities;

public class GeneralLedger
{
    public Guid Id { get; private set; } // Use GUID for better uniqueness
    public AccountNumber AccountNumber { get; private set; }
    public string AccountName { get; private set; }
    public decimal Balance { get; private set; }
    public decimal MinimumBalance { get; private set; } = 0m; // Default minimum balance
    public bool CanRunNegative { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public string Description { get; private set; }
    public CurrencyType Currency { get; private set; }

    // Navigation property
    public ICollection<GlTransactions> Audits { get; private set; } = new List<GlTransactions>();

    // Constructor
    public GeneralLedger(
        string accountName,
        CurrencyType currency,
        string description,
        bool canRunNegative = false,
        decimal minimumBalance = 0m)
    {
        Id = Guid.NewGuid();
        AccountNumber = AccountNumber.Create(currency, accountName);
        AccountName = accountName;
        Currency = currency;
        Description = description;
        CanRunNegative = canRunNegative;
        MinimumBalance = minimumBalance;
        Balance = 0m; // Initial balance is zero
    }

    // Business logic: Update balance
    public void UpdateBalance(decimal amount)
    {
        if (!CanRunNegative && Balance + amount < MinimumBalance)
            throw new InvalidOperationException("Insufficient balance to perform this operation.");

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
    }

}