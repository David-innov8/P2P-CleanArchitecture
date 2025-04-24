using P2P.Application.UseCases.Interfaces;

namespace P2P.Infrastructure.Services;

public class AccountNumberGenerator:IAccountNumberGenerator
{
    private readonly Random _random = new Random();
    private readonly IAccountRepository _accountRepository;

    public AccountNumberGenerator(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    // public string GenerateAccountNumber()
    // {
    //     // Generate a 10-digit account number
    //     // You might want to implement more sophisticated logic based on your requirements
    //     return $"AC{DateTime.UtcNow.ToString("yyyyMMdd")}{_random.Next(1000, 9999)}";
    // }
    public string GenerateAccNo()
    {
        // Generate account numbers starting from this value
        // Generate account numbers starting from this value
        for (int baseNumber = 200000000; baseNumber < 200000100; baseNumber++) // Adjust the range as needed
        {
            string number = baseNumber.ToString();
            char check;

            if (TryCalculateMod11CheckChar(number, out check))
            {
                string fullAccountNumber = number + check;

                // Check for uniqueness in the database
            var AccExists =  _accountRepository.AccountNumberExistsAsync(fullAccountNumber);
            if(AccExists.Result ==false)
            {
                return fullAccountNumber;
            }
            }
        }

        throw new InvalidOperationException("Unable to generate a unique account number.");
    }

    private bool TryCalculateMod11CheckChar(string number, out char checkDigit)
    {
        int factor = number.Length + 1;
        int sum = 0;
        for (int i = 0; i < number.Length; i++)
        {
            int cval = number[i] - '0';
            sum += cval * factor;
            factor--;
        }

        int delta = sum % 11;
        if (delta == 1)
        {
            checkDigit = '!';
            return false;
        }
        else
        {
            checkDigit = (delta == 0) ? '0' : (11 - delta).ToString()[0];
            return true;
        }
    }
}
