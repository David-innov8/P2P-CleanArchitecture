using P2P.Application.UseCases.Interfaces;

namespace P2P.Infrastructure.Services;

public class AccountNumberGenerator:IAccountNumberGenerator
{
    private readonly Random _random = new Random();
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork; 
    private static int _lastSequence;
    private static readonly object _lockObject = new object();
    private Queue<string> _availableAccountNumbers = new Queue<string>();
    private const int BATCH_SIZE = 50;
    public AccountNumberGenerator(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _accountRepository = accountRepository;
        
        InitializeSequence();
    }

    private void InitializeSequence()
    {
        if (_lastSequence == 0)
        {
            
            // The lock statement is used for thread synchronization - 
            //     it ensures that only one thread can execute a block of code at a time, preventing race conditions in multi-threaded applications.
            lock (_lockObject)
            {
                if (_lastSequence == 0)
                {
                    string highestAccount = _accountRepository.GetHighestAccountNumberAsync().GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(highestAccount) && highestAccount.Length >= 9)
                    {
                        // Try to parse the account number (minus the last check digit)
                        string sequencePart = highestAccount.Substring(0, highestAccount.Length - 1);
                        if (int.TryParse(sequencePart, out int sequence))
                        {
                            _lastSequence = sequence;
                        }
                        else
                        {
                            // Default starting point if we can't parse
                            _lastSequence = 200000000;
                        }
                    }
                    else
                    {
                        // Default starting point if no accounts exist
                        _lastSequence = 200000000;
                    }
                }
            }
        }
    }
    
    public string GenerateAccNo()
    {
        // If we don't have any pre-validated account numbers, generate a batch
        if (_availableAccountNumbers.Count == 0)
        {
            RefillAccountNumbersCache();
        }
        
        // Return the next available pre-validated account number
        return _availableAccountNumbers.Dequeue();
    }
    
    private void RefillAccountNumbersCache()
    {
        List<string> candidateNumbers = new List<string>();
        int attempts = 0;
        
        // Generate a batch of candidate account numbers
        while (candidateNumbers.Count < BATCH_SIZE && attempts < BATCH_SIZE * 2)
        {
            int nextSequence;
            lock (_lockObject)
            {
                _lastSequence++;
                nextSequence = _lastSequence;
            }
            
            // Ensure the number starts with 200
            if (nextSequence < 200000000)
            {
                nextSequence = 200000000;
            }
            
            string baseNumber = nextSequence.ToString();
            if (TryCalculateMod11CheckChar(baseNumber, out char checkDigit))
            {
                candidateNumbers.Add(baseNumber + checkDigit);
            }
            
            attempts++;
        }
        
        // Filter out numbers that already exist in the database (one database call)
        List<string> uniqueNumbers = FilterExistingNumbers(candidateNumbers);
        
        // Add the unique numbers to our cache
        foreach (var number in uniqueNumbers)
        {
            _availableAccountNumbers.Enqueue(number);
        }
        
        // If we couldn't generate any valid numbers, throw an exception
        if (_availableAccountNumbers.Count == 0)
        {
            throw new InvalidOperationException("Unable to generate unique account numbers.");
        }
    }
    
    private List<string> FilterExistingNumbers(List<string> candidateNumbers)
    {
        // Get a list of numbers that don't exist in the database
        List<string> uniqueNumbers = new List<string>();
        
        // This method makes a single database call to check multiple numbers
        var existingNumbersMap = _accountRepository.CheckAccountNumbersExistAsync(candidateNumbers)
            .GetAwaiter().GetResult();
        
        foreach (var number in candidateNumbers)
        {
            if (!existingNumbersMap.ContainsKey(number) || !existingNumbersMap[number])
            {
                uniqueNumbers.Add(number);
            }
        }
        
        return uniqueNumbers;
    }
    // public string GenerateAccNo()
    // {
    //     // Generate account numbers starting from this value
    //     // Generate account numbers starting from this value
    //     for (int baseNumber = 200000000; baseNumber < 200000100; baseNumber++) // Adjust the range as needed
    //     {
    //         string number = baseNumber.ToString();
    //         char check;
    //
    //         if (TryCalculateMod11CheckChar(number, out check))
    //         {
    //             string fullAccountNumber = number + check;
    //
    //             // Check for uniqueness in the database
    //         var AccExists =  _accountRepository.AccountNumberExistsAsync(fullAccountNumber);
    //         if(AccExists.Result ==false)
    //         {
    //             return fullAccountNumber;
    //         }
    //         }
    //     }
    //
    //     throw new InvalidOperationException("Unable to generate a unique account number.");
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
