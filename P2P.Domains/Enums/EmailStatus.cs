namespace P2P.Domains;

public enum EmailStatus
{

  
        Pending = 0,
        Processing = 1,
        Sent = 2,
        Failed = 3,
        DeadLetter = 4 // Permanently failed after max retries

}