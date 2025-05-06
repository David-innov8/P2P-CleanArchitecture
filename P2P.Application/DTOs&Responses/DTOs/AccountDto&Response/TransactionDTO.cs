namespace P2P.Application.DTOs.AccountDto_Response;

public class TransactionDTO
{
    public Guid TransactionId { get; set; }
    public string AccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string RecipientAccountNumber { get; set; }
    public string Reference { get; set; }
}