using P2P.Application.DTOs.USerDTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.UserAccounts;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases;

public class GetReceipeintDetailsUseCase:IGetReciepientDetailsUSeCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    public GetReceipeintDetailsUseCase(IAccountRepository accountRepository, IUserRepository userRepository)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
    }
    
    
    
    public async Task<ApiResponse<ReciepientDetailsDTo>> ExecuteAsync(string accountNoOrUsername)
    {
        if (string.IsNullOrEmpty(accountNoOrUsername))
        {
            return  ApiResponse<ReciepientDetailsDTo>.FailedResponse(null, "Account number or username is required");
        }

        bool isAccountNumber = int.TryParse(accountNoOrUsername, out int accountNumber);
        Console.WriteLine(isAccountNumber);

        if (isAccountNumber)
        {
            var account = await _accountRepository.GetAccountByNumberAsync(accountNoOrUsername);
            // var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNoOrUsername);
            if (account != null)
            {
                // var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == account.UserId);
                var user = await _userRepository.GetByIdAsync(account.Id);
                if (user != null)
                {
                    var res = new ReciepientDetailsDTo
                    {
                        FirstName = user.Profile.FirstName,
                        LastName = user.Profile.LastName,
                    };
                    return  ApiResponse<ReciepientDetailsDTo>.SuccessResponse(res, "Account found" );
                }
             
            }

        }
        else
        {
            var user = await _userRepository.GetUserByUsernameAsync(accountNoOrUsername);
            if (user != null)
            {
                var res = new ReciepientDetailsDTo
                {
                    FirstName = user.Profile.FirstName,
                    LastName = user.Profile.LastName,
                };

                return  ApiResponse<ReciepientDetailsDTo>.SuccessResponse(res, "User found");
            }
        }




        return  ApiResponse<ReciepientDetailsDTo>.FailedResponse(null, "no valid Input provided");
    }
}