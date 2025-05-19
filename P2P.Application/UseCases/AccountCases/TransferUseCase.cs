using P2P.Application.DTOs.AccountDto_Response;
using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.GeneralLedgers;
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
    private readonly IGLRepository _glRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGLTransactionRepository _glTransactionRepository;
    public TransferUseCase(
        IUserRepository userRepository,
        IAccountRepository accountRepository, ITransactionsRepository transactionRepository,    IGLRepository glRepository, IUnitOfWork unitOfWork,IGLTransactionRepository glTransactionRepository)
   
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _glRepository = glRepository;
        _unitOfWork = unitOfWork;
        _glTransactionRepository = glTransactionRepository;
    }


    public async Task<ApiResponse<TransferDTO>> ExecuteTransfer(string recipientUsername, decimal amount,
        CurrencyType currency)
    {
        // Step 1: Validate sender
        var sender = await _userRepository.GetUserWithAccountsFromClaimsAsync();
        if (sender == null || string.IsNullOrEmpty(sender.Username))
            throw new UserDoesntExistException("Sender not found or incomplete data.");

        // Step 2: Validate recipient
        var recipient = await _userRepository.GetUserWithAccountsByUsernameAsync(recipientUsername);
        if (recipient == null || string.IsNullOrEmpty(recipient.Username))
            throw new UserDoesntExistException("Recipient not found ");

        if (sender.Username.Equals(recipient.Username, StringComparison.OrdinalIgnoreCase))
        {
            return ApiResponse<TransferDTO>.FailedResponse(null, "You cannot send money to yourself.");
        }
        
        // Step 3: Find sender's and recipient's accounts with matching currency
        var senderAccount = sender.Accounts.FirstOrDefault(a => a.Currency == currency);
     
        var recipientAccount = recipient.Accounts.FirstOrDefault(a => a.Currency == currency);

        if (senderAccount == null || recipientAccount == null)
            throw new AccountNotFoundException("No matching accounts found for the specified currency.");

        // Step 4: Verify sender's PIN
        // if (!sender.Pin.Verify(pin))
        //     throw new InvalidPinException("Invalid PIN.");

        
        var transferGL = await _glRepository.GetSystemGLByTypeAndCurrencyAsync(GLType.TransferGL, currency);
        var depositGL = await _glRepository.GetSystemGLByTypeAndCurrencyAsync(GLType.DepositGL, currency);

        if (transferGL == null || depositGL == null)
            throw new Exception("Required general ledgers not found for this currency.");

        // Generate a unique reference for this transaction
        string transactionReference = $"TRF-{Guid.NewGuid().ToString().Substring(0, 8)}";
        // Step 5: Perform the transfer
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            senderAccount.Transfer(recipientAccount, amount);

            transferGL.UpdateBalance(amount);
             _glRepository.Update(transferGL);
            
   
            depositGL.UpdateBalance(-amount);
             _glRepository.Update(depositGL);


     
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

            await _transactionRepository.AddAsync(senderTransaction);
            await _transactionRepository.AddAsync(recipientTransaction);

            var transferGLTransaction = new GlTransactions(
                transferGL.Id, 
                sender.Id, 
                senderTransaction.TransactionId, 
                amount, 
                currency, 
                TransactionType.Credit,
                TransactionStatus.Completed
                
            );
            
            var depositGLTransaction = new GlTransactions(
                depositGL.Id, 
                sender.Id, 
                senderTransaction.TransactionId, 
                -amount, 
                currency,
                TransactionType.Credit, 
                TransactionStatus.Completed
         
            );
            
            await _glTransactionRepository.AddAsync(transferGLTransaction);
            await _glTransactionRepository.AddAsync(depositGLTransaction);

        await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            var transferDto = new TransferDTO
            {
                TransactionId = senderTransaction.TransactionId,
                Amount = amount,
                Currency = currency,
                Status = TransactionStatus.Completed
            };

            return ApiResponse<TransferDTO>.SuccessResponse(transferDto, "Transfer successful.");

        }
        catch (InsufficientFundsException ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<TransferDTO>.FailedResponse(null, "Insufficient funds.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<TransferDTO>.FailedResponse(null, "Transfer failed.");
        }
    }
}