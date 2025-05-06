namespace P2P.Application.DTOs.AccountDto_Response;

public class PagedTransactionResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<TransactionDTO> Transactions { get; set; }
}