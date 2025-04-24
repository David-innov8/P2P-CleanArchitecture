namespace P2P.Application.UseCases.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password, out byte[] passwordSalt, out byte[] passwordHash);
    bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash);
}