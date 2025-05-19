using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces;

public interface IUserRepository  :IRepository<User>
{
    Task<User> GetByIdAsync(Guid userId);
   Task<User> GetUserByEmailAsync(string email);
   Task<User> GetUserByUsernameAsync(string username);
   Task<User> GetUserFromClaimsAsync();
    Task AddUserAsync(User user);

    Task UpdateUserAsync(User user);

    Task<User> GetUserWithAccountsFromClaimsAsync();

    Task<User> GetUserWithAccountsByUsernameAsync(string username);
    
    
// Task<string> GetUserEmailFromClaimsAsync();
}