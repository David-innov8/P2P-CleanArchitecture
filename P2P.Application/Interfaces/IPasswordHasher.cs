namespace P2P.Application.UseCases.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password, out byte[] passwordSalt, out byte[] passwordHash);

    Task<(string hash, byte[] salt)> HashPasswordAsync(string password);
    
    
    bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash);
}