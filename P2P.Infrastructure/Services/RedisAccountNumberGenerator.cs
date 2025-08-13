using P2P.Application.UseCases.Interfaces;
using StackExchange.Redis;

namespace P2P.Infrastructure.Services;

public class RedisAccountNumberGenerator: IAccountNumberGenerator
{
    private readonly IDatabase _redis;
    private const string ACCOUNT_KEY = "account_number_counter";
    private const int MIN_SEQUENCE = 200000000; // Start at 200000000
    private const int MAX_SEQUENCE = 200999999;
    private const int CHECK_DIGIT_BASE = 11;

    public RedisAccountNumberGenerator(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }
    
    
    public async Task<string> GenerateAccNoAsync()
    {
        long nextSequence = await _redis.StringIncrementAsync(ACCOUNT_KEY);

        // Ensure we start at MIN_SEQUENCE if the key was empty
        if (nextSequence < MIN_SEQUENCE)
        {
            nextSequence = MIN_SEQUENCE;
            await _redis.StringSetBitAsync(ACCOUNT_KEY, 0, true); // Force set bit
        }

        // Wrap around when reaching max
        if (nextSequence > MAX_SEQUENCE)
        {
            // Reset back to MIN_SEQUENCE
            await _redis.KeyDeleteAsync(ACCOUNT_KEY);
            nextSequence = MIN_SEQUENCE;
        }

        string baseNumber = nextSequence.ToString();

        // Add MOD11 check digit
        if (!TryCalculateMod11CheckChar(baseNumber, out char checkDigit))
        {
            throw new InvalidOperationException("Unable to calculate valid check digit.");
        }

        return baseNumber + checkDigit;
    }
    // private bool TryCalculateMod11CheckChar(string number, out char checkDigit)
    // {
    //     int factor = number.Length + 1;
    //     int sum = 0;
    //
    //     for (int i = 0; i < number.Length; i++)
    //     {
    //         int cval = number[i] - '0';
    //         sum += cval * factor;
    //         factor--;
    //     }
    //
    //     int delta = sum % CHECK_DIGIT_BASE;
    //
    //     if (delta == 1)
    //     {
    //         checkDigit = '!';
    //         return false;
    //     }
    //
    //     checkDigit = (delta == 0) ? '0' : (char)('0' + (CHECK_DIGIT_BASE - delta));
    //     return true;
    // }

    
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

        // Handle special cases to ensure we get a valid digit
        if (delta <= 1)
        {
            // Use 0 or 1 depending on spec
            checkDigit = (char)('0' + (11 - delta) % 10);
        }
        else
        {
            checkDigit = (char)('0' + (11 - delta));
        }

        return true;
    }
}