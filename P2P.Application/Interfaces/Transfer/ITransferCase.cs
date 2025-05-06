using P2P.Application.DTOs.AccountDto_Response;
using P2P.Domains;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.Transfer;

public interface ITransferCase
{
    Task<ApiResponse<TransferDTO>> ExecuteTransfer(string recipientUsername, decimal amount, CurrencyType currency);
}