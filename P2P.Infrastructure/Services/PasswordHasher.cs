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

    public bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash)
    {
        if (passwordSalt == null || passwordHash == null)
            return false;

        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}