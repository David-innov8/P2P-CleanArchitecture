using P2P.Application.DTOs.AccountDto_Response;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.AccountCases;

public class GetUserAccountDetails:IGetUserAccountDetails
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;

    public GetUserAccountDetails(IUserRepository userRepository, IAccountRepository accountRepository)
    {
     _userRepository = userRepository;
     _accountRepository = accountRepository;
    }

  public async  Task<ApiResponse<UserAccountsResponse>> GetUserAccountInfo()
  {
      var userAccounts =await  _userRepository.GetUserWithAccountsFromClaimsAsync();

      var accountInfo = userAccounts.Accounts.Select(a => new AccountDTO()
      {
          AccountNumber = a.AccountNumber,
          Balance = a.Balance,
          Currency = a.Currency.ToString(),
      }).ToList();

      var response = new UserAccountsResponse
      {
          Username = userAccounts.Username,
          Accounts = accountInfo
      };

      return  ApiResponse<UserAccountsResponse>.SuccessResponse(response, " user account details fetched succesfully.");
  }
}