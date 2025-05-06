using P2P.Domains.Exceptions;

namespace P2P.Domains.Entities;

public class Account
{
    public Guid Id { get; private set; }
    
    public string AccountNumber { get; private set; }
    public decimal Balance { get; private set; }
    public CurrencyType Currency { get; private set; }

    // Foreign key to User
    public Guid UserId { get; private set; }

    public Account(Guid userId, string accountNumber, CurrencyType currency )
    {
        Id = Guid.NewGuid();
        UserId = userId;
        AccountNumber = accountNumber;
        Currency = currency;
        if (currency == CurrencyType.NGN)
        {
            Balance = 10000;
        }
        Balance = 0;
        
    }

    public void Deposit(decimal amount)
    {
        if(amount <= 0)
            throw  new NegativeDepositException("Deposit amount cannot be negative");

        Balance += amount;
        
    }

//basically send money 
    public void Withdraw(decimal amount)
    {
        if(amount <= 0)
            throw new NegativeDepositException("withdrawal amount cannot be negative");
        if(amount > Balance)
                throw new InsufficientFundsException("insufficient funds");
        
        Balance -= amount;
            
        
    }

    public void Transfer( Account toAccount, decimal amount)
    {
        if (toAccount == null)
            throw new AccountNotFoundException("Recipient account not found.");

        if (this.Currency != toAccount.Currency)
            throw new CurrencyMismatchException("Currency mismatch between accounts.");
        if (this == toAccount)
            throw new InvalidOperationException("Cannot transfer to the same account.");


        if (amount <= 0)
            throw new NegativeDepositException("Transfer amount must be positive.");

        if (amount > Balance)
            throw new InsufficientFundsException("Insufficient funds in the sender's account.");

        Withdraw(amount);
        toAccount.Deposit(amount);
            
        
    }
}