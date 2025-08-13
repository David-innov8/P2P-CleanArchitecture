using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using P2P.Application.UseCases.Interfaces;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Services;

public class PasswordHasher:IPasswordHasher
{
    private readonly P2pContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PasswordHasher(P2pContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string HashPassword(string password, out byte[] passwordSalt, out byte[] passwordHash)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    
            return Convert.ToBase64String(passwordHash);
        }
    }
    
    public async Task<(string hash, byte[] salt)> HashPasswordAsync(string password)
    {
        byte[] salt = new byte[16]; // 128-bit salt
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pdb = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pdb.GetBytes(32); // 256-bit hash

        return (Convert.ToBase64String(hash), salt);
    }
    
    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }

    public bool VerifyPassword(string password, byte[] storedSalt, byte[] storedHash)
    {
        if (storedSalt == null || storedHash == null)
            return false;

        var pdb = new Rfc2898DeriveBytes(password, storedSalt, 10000, HashAlgorithmName.SHA256);
        byte[] computedHash = pdb.GetBytes(32);

        return SlowEquals(computedHash, storedHash);
    }
}