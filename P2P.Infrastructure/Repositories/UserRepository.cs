using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;
   private readonly P2pContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository( P2pContext context ,IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor):base(context)
    {
        _unitOfWork = unitOfWork;
      
        _httpContextAccessor = httpContextAccessor;
    }



    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await Find(u=>u.Id == userId);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await Find(u => u.Email == email);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await Find(u => u.Username == username);
    }

 
    public async Task<User> GetUserWithAccountsByUsernameAsync(string username)
    {
        return await _unitOfWork
            .GetRepository<User>()
            .Query() 
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> GetUserFromClaimsAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new Exception("User ID claim not found.");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new Exception("Invalid user ID format in claim.");

        var user = await Find(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found.");

        return user;
    }
    
    public async Task<User> GetUserWithAccountsFromClaimsAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new Exception("Invalid user claim.");

        return await _unitOfWork
            .GetRepository<User>()
            .Query()
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<string> GetUserEmailFromClaimsAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            throw new Exception("User ID claim not found.");


        var user = await GetUserByEmailAsync(email);


        if (user == null)
            throw new Exception("User not found.");

        return user.Email;
    }

    public async Task AddUserAsync(User user)
    {
        await AddAsync(user);
 
    }
    
    public async Task UpdateUserAsync(User user)
    {
        Update(user);

    }
    
   
}