using P2P.Application.DTOs.AccountDto_Response;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains.Entities;
using P2P.Domains.Exceptions;

namespace P2P.Application.UseCases.AccountCases;

public class GetTransactionHistoryUseCase: ITransactionHistory
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionsRepository _transactionRepository;

    public GetTransactionHistoryUseCase(
        IUserRepository userRepository,
        ITransactionsRepository transactionRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<ApiResponse<PagedTransactionResponse>> GetTransactionsByUserIdAsync( int pageNumber, int pageSize)
    {
        try
        {
            var user = await _userRepository.GetUserWithAccountsFromClaimsAsync();
            if (user == null)
                return ApiResponse<PagedTransactionResponse>.FailedResponse(null, "User not found");

            // Fetch paginated transactions
            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(user.Id, pageNumber, pageSize);

            var transactionDtos = transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType.ToString(),
                Status = t.TransactionType.ToString(),
                TransactionDate = t.TransactionDate,
            
            }).ToList();
            var response = new PagedTransactionResponse
            {
                Transactions = transactionDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = await _transactionRepository.CountTransactionsByUserIdAsync(user.Id)
            };

            return ApiResponse<PagedTransactionResponse>.SuccessResponse(response, "Transactions retrieved successfully");
        }
        catch
        {
            return ApiResponse<PagedTransactionResponse>.FailedResponse(null,"Unable to fetch transactions by user");
        }
    }
}