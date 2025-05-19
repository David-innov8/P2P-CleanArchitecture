using P2P.Domains;

namespace P2P.Application.DTOs;

public class TransferRequestDto
{
    public string RecipientUsername { get; set; }
    public decimal Amount { get; set; }
    public CurrencyType Currency { get; set; }
    // public string Pin { get; set; }
}