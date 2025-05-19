using System.Text;

namespace P2P.Domains.ValueObjects;

public record GLAccountNumber(string value)
{
    public override string ToString() => value;
    public static GLAccountNumber Create(CurrencyType currency, GLType glType, string accountName)
    {
        // Format: GL-{CURRENCY}-{GLTYPE}-{SANITIZED_NAME}-{CHECKSUM}
        var sanitizedAccountName = SanitizeAccountName(accountName);
        var currencyCode = currency.ToString().ToUpper().Substring(0, Math.Min(3, currency.ToString().Length));
        var glTypeCode = glType.ToString().ToUpper().Substring(0, Math.Min(3, glType.ToString().Length));
        
        // Create a deterministic but unique account number
        var baseString = $"{currencyCode}{glTypeCode}{sanitizedAccountName}";
        var checksum = CalculateChecksum(baseString);
        
        var accountNumber = $"GL-{currencyCode}-{glTypeCode}-{sanitizedAccountName}-{checksum}";
        return new GLAccountNumber(accountNumber);
    }

    private static string SanitizeAccountName(string accountName)
    {
        // Remove any non-alphanumeric characters
        var sanitized = new StringBuilder();
        foreach (char c in accountName)
        {
            if (char.IsLetterOrDigit(c))
                sanitized.Append(c);
        }
        
        // Take first 5 characters or pad if shorter
        var result = sanitized.ToString().ToUpper();
        if (result.Length > 5)
            return result.Substring(0, 5);
        
        return result.PadRight(5, '0');
    }
    
    private static string CalculateChecksum(string input)
    {
        // Simple checksum algorithm: sum of ASCII values modulo 9999
        int sum = 0;
        foreach (char c in input)
        {
            sum += (int)c;
        }
        
        return (sum % 9999).ToString().PadLeft(4, '0');
    }

  
}
