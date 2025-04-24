namespace P2P.Domains.ValueObjects;

public record AccountNumber(string value)
{
    public static AccountNumber Create(CurrencyType currency, string accountName)
    {
        var sanitizedAccountName = accountName.Substring(0, Math.Min(3, accountName.Length)).ToUpper();
        var random = new Random();
        var randomDigits = random.Next(1000, 9999).ToString();

        var accountNumber = $"{currency.ToString().ToUpper()}-{sanitizedAccountName}-{randomDigits}";
        return new AccountNumber(accountNumber);
    }

    public override string ToString() => value;
}