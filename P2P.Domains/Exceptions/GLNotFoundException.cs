namespace P2P.Domains.Exceptions;

public class GLNotFoundException:Exception
{
    public GLNotFoundException() : base() { }
    public GLNotFoundException(string message) : base(message) { }
    public GLNotFoundException(string message, Exception innerException) : base(message, innerException) { }

}