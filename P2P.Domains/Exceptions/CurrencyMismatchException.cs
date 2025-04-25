namespace P2P.Domains.Exceptions;

public class CurrencyMismatchException:Exception
{
    public CurrencyMismatchException(string message) : base(message){}
}