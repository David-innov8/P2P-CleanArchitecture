using P2P.Application.DTOs.Paystack;
using P2P.Application.DTOs.PaystackDtosandResponse;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.Paystack;

public interface IPaystackService
{
    Task<ApiResponse<PaystackTransactionResponse>> InitializePayment( decimal amount );
    Task<ApiResponse<string>> VerifyPayment(string reference);
    Task<ApiResponse<string>> HandleWebhook(string reference, string eventType, decimal amount);

}