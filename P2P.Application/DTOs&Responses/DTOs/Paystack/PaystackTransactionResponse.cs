namespace P2P.Application.DTOs.Paystack;

public class PaystackTransactionResponse
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public PaystackData Data { get; set; }
}