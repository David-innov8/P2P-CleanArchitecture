namespace P2P.Domains.Exceptions;

public class AccountNotFoundException: Exception
{
    public AccountNotFoundException(string message) : base(message){}
}