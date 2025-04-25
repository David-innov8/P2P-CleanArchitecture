using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly P2pContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(P2pContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }



    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u=> u.Id == userId);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.Include(u=>u.Accounts).FirstOrDefaultAsync(u => u.Email == email);
    }

   public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.Include(u=>u.Accounts).FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> GetUserFromClaimsAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new Exception("User ID claim not found.");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new Exception("Invalid user ID format in claim.");

        var user = await _context.Users
            .Include(u => u.Accounts) // Include the Accounts navigation property
            .FirstOrDefaultAsync(u => u.Id == userId);


        if (user == null)
            throw new Exception("User not found.");

        return user;
    }

    public async Task<string> GetUserEmailFromClaimsAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new Exception("User ID claim not found.");

     
        var user = await _context.Users.FindAsync(userIdClaim);

        if (user == null)
            throw new Exception("User not found.");

        return user.Email;
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}