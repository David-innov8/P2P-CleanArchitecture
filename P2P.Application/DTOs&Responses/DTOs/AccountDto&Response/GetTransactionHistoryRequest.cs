namespace P2P.Application.DTOs.AccountDto_Response;

public class GetTransactionHistoryRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}