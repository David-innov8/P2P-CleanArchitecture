namespace P2P.Application.DTOs.PaystackDtosandResponse;

public class PaystackTransactionResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public PaystackData Data { get; set; }
}