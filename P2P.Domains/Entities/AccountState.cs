namespace P2P.Domains.Entities;

public class AccountState
{
    public bool IsAccountLocked { get; private set; }
    public int RetryCount { get; private set; }

    public void lockAccount()
    {
        if (RetryCount >= 5)
        {
            IsAccountLocked = true;
        }
    }
    
    public void ResetRetryCount() => RetryCount = 0;

    public void IncrementRetryCount()
    {
        RetryCount++;
        if (RetryCount >= 3)
        {
            lockAccount();
        }
    }
}