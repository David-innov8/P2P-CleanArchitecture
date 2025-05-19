using P2P.Application.DTOs.PaystackDtosandResponse;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.Paystack;

public interface IPaystackService
{
    Task<ApiResponse<PaystackTransactionResponseDto>> InitializePayment(FundAccountRequestDto request);
    Task<ApiResponse<string>> VerifyPayment(string reference);
    Task<ApiResponse<string>> HandleWebhook(string reference, string eventType, decimal amount);

}