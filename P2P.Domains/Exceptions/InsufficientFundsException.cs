namespace P2P.Domains.Exceptions;

public class InsufficientFundsException: Exception
{
    public InsufficientFundsException(string message) : base(message){}
}