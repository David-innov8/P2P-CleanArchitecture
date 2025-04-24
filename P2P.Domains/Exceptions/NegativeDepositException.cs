namespace P2P.Domains.Exceptions;

public class NegativeDepositException : Exception
{
    public NegativeDepositException(string message) : base(message){}
}