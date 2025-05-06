using P2P.Application.DTOs.AccountDto_Response;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.Transfer;

public interface ITransactionHistory
{
    Task<ApiResponse<PagedTransactionResponse>> GetTransactionsByUserIdAsync(int pageNumber, int pageSize);
}