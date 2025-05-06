namespace P2P.Domains.Exceptions;

public class TransferFailedException:Exception
{
    public TransferFailedException(string message) : base(message)
    {
    }

    public TransferFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}