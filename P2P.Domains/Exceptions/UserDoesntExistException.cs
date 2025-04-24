namespace P2P.Domains.Exceptions;

public class UserDoesntExistException: Exception
{
    public UserDoesntExistException(string message): base(message)
    {
        
    }
}