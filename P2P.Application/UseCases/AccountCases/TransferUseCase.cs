using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains;
using P2P.Domains.Entities;
using P2P.Domains.Exceptions;

namespace P2P.Application.UseCases.AccountCases;

public class TransferUseCase:ITransferCase
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionsRepository _transactionRepository;
    
    public TransferUseCase(
        IUserRepository userRepository,
        IAccountRepository accountRepository, ITransactionsRepository transactionRepository)
   
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }


    public async Task ExecuteTransfer(string recipientUsername, decimal amount, CurrencyType currency)
{
    // Step 1: Validate sender
    var sender = await _userRepository.GetUserFromClaimsAsync();
    if (sender == null || string.IsNullOrEmpty(sender.Username))
        throw new UserDoesntExistException("Sender not found or incomplete data.");

    // Step 2: Validate recipient
    var recipient = await _userRepository.GetUserByUsernameAsync(recipientUsername);
    if (recipient == null || string.IsNullOrEmpty(recipient.Username))
        throw new UserDoesntExistException("Recipient not found or incomplete data.");

    // Step 3: Find sender's and recipient's accounts with matching currency
    var senderAccount = sender.Accounts.FirstOrDefault(a => a.Currency == currency);
    var recipientAccount = recipient.Accounts.FirstOrDefault(a => a.Currency == currency);

    if (senderAccount == null || recipientAccount == null)
        throw new AccountNotFoundException("No matching accounts found for the specified currency.");

    // Step 4: Verify sender's PIN
    // if (!sender.Pin.Verify(pin))
    //     throw new InvalidPinException("Invalid PIN.");

    // Step 5: Perform the transfer
    try
    {
        senderAccount.Transfer(senderAccount, recipientAccount, amount);

        // Step 6: Record the transaction
        var senderTransaction = new Transactions(
            senderAccount.Id,
            -amount,
            currency,
            TransactionType.Debit,
            $"Transfer to {recipient.Username}"
        );

        var recipientTransaction = new Transactions(
            recipientAccount.Id,
            amount,
            currency,
            TransactionType.Credit,
            $"Transfer from {sender.Username}"
        );

        _transactionRepository.Add(senderTransaction);
        _transactionRepository.Add(recipientTransaction);
    }
    catch (InsufficientFundsException ex)
    {
        throw new InsufficientFundsException("Insufficient funds to complete the transfer.");
    }
}
}