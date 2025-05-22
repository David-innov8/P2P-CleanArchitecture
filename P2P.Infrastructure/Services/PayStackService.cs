using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using P2p_Clean_Architecture________b;
using P2P.Application.DTOs.Paystack;
using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.Paystack;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains;
using P2P.Domains.Entities;

namespace P2P.Infrastructure.Services;

public class PayStackService: IPaystackService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IGLRepository _glRepository;
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PayStackService(IUserRepository userRepository, IAccountRepository accountRepository, HttpClient httpClient, AppSettings appSettings, ITransactionsRepository transactionsRepository, IGLRepository glRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _httpClient = httpClient;
        _appSettings = appSettings;
        _transactionsRepository = transactionsRepository;
        _glRepository = glRepository;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        
        
        _httpClient.BaseAddress = new Uri(_appSettings.PaystackBaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _appSettings.PaystackSecretKey);

    }
    
    public async Task<ApiResponse<PaystackTransactionResponse>>  InitializePayment(decimal amount )
    {

         

        try
        {
            
            
           var currentUserIdString =  _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
           if (string.IsNullOrEmpty(currentUserIdString) || !Guid.TryParse(currentUserIdString, out var currentUserId))
           {
               return ApiResponse<PaystackTransactionResponse>.FailedResponse(null, "Invalid user ID");
           }
           var currentUser = await _userRepository.GetByIdAsync(currentUserId);            if (currentUser == null)
            {
                return ApiResponse<PaystackTransactionResponse>.FailedResponse(null, "User not found");
            }
            var userAccounts = await _accountRepository.GetAccountsByUserIdAsync(currentUserId);
            var currentUserAccount = userAccounts.FirstOrDefault();
            if (currentUserAccount == null)
            {
                return ApiResponse<PaystackTransactionResponse>.FailedResponse(null, "Account not found");
            }
            
            var paystackRequest = new
            {
                email = currentUser.Email,
                amount = amount * 100, // Convert to kobo
                reference = GenerateReference(),
                // callback_url = "https://yourwebsite.com/verify-payment",
                metadata = new
                {
                    accountId = currentUserAccount.Id,
                    accountNumber = currentUserAccount.AccountNumber
                }
            };
            
            var response = await _httpClient.PostAsJsonAsync("https://api.paystack.co/transaction/initialize", paystackRequest);
            if (!response.IsSuccessStatusCode)
            {
                return  ApiResponse<PaystackTransactionResponse>.FailedResponse(null, "Failed to initialize payment");
            }
            
            
            var paystackResponse = await response.Content.ReadFromJsonAsync<PaystackTransactionResponse>();
            
            var transaction = new Transactions(currentUserAccount.Id, amount, CurrencyType.NGN, TransactionType.Credit,$"Paystack deposit - {paystackResponse.Data.Reference}", TransactionStatus.Pending, paystackResponse.Data.Reference);
         
            await _transactionsRepository.AddAsync(transaction);
            
            return ApiResponse<PaystackTransactionResponse>.SuccessResponse(paystackResponse,"Payment Initialize successfully");
        }
        catch(Exception ex)
        {
            return ApiResponse<PaystackTransactionResponse>.FailedResponse(null, $"Failed to initialize payment: {ex.Message}");
        }
    }
   
    
    private string GenerateReference()
    {
        return $"TR-{DateTime.Now.Ticks}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }

    public async Task<ApiResponse<string>> VerifyPayment(string reference)
    {
        try
        {
            var transaction = await _transactionsRepository.QueryWithIncludes("Account").FirstOrDefaultAsync(t => t.PaystackReference == reference && t.Status == TransactionStatus.Pending);

            if (transaction == null)
            {
                return ApiResponse<string>.FailedResponse("Invalid reference");
            }
            
            var response = await _httpClient.GetAsync($"/transaction/verify/{reference}");
            if (!response.IsSuccessStatusCode)
            {
                return  ApiResponse<string>.FailedResponse(null, "Failed to verify payment");
            }
            
            var verificationResponse = await response.Content.ReadFromJsonAsync<PaystackTransactionResponse>();
            
            if (!verificationResponse.Status)
            {
                return  ApiResponse<string>.FailedResponse(null, "Payment verification failed");
            }
            
            try
            {
                await  _transactionsRepository.UpdateStatusAsync(transaction.TransactionId, TransactionStatus.Completed);
           
                
                transaction.Account.Deposit(transaction.Amount); 
            
              _transactionsRepository.Update(transaction);
              await _unitOfWork.SaveChangesAsync(); 
                return  ApiResponse<string>.SuccessResponse(null, "Payment verified and account credited");
            }
            catch (Exception ex)
            {
              
                return  ApiResponse<string>.FailedResponse(null, $"Error updating account: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.FailedResponse($"Failed to verify payment: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> HandleWebhook(string reference, string eventType, decimal amount)
    {
        var userId = _userRepository.GetUserFromClaimsAsync();
        if (eventType != "charge.success")
        {
            return  ApiResponse<string>.FailedResponse(null, "Invalid event type");
        }

        try
        {
            var transaction = await _transactionsRepository.Query().Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.PaystackReference == reference);

            if (transaction == null)
            {
                return ApiResponse<string>.FailedResponse(null, "Transaction not found");
            }

            int transactionAmountInKobo = (int)(transaction.Amount * 100);
            if (transactionAmountInKobo != amount)
            {
                return ApiResponse<string>.FailedResponse(null, "Transaction amount does not match");
            }


            TransactionStatus newStatus = eventType == "charge.success"
                ? TransactionStatus.Completed
                : TransactionStatus.Failed;

            transaction.UpdateStatus(newStatus);

            var userAccounts = await _accountRepository.GetByIdAsync(transaction.AccountId);

            if (userAccounts == null)
            {
                return ApiResponse<string>.FailedResponse(null, "User account not found");
            }

            var depositGL = await _glRepository.GetSystemGLByTypeAndCurrencyAsync(GLType.DepositGL, CurrencyType.NGN);

            var transferGl = await _glRepository.GetSystemGLByTypeAndCurrencyAsync(GLType.TransferGL, CurrencyType.NGN);
            if (transferGl == null || depositGL == null)
            {
                return ApiResponse<string>.FailedResponse(null, "GL Account not found");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                userAccounts.Deposit(amount);

                transferGl.UpdateBalance(amount);
                _glRepository.Update(transferGl);


                depositGL.UpdateBalance(-amount);
                _glRepository.Update(depositGL);

                var recipientTransaction = new Transactions(
                    userId.Result.Id,
                    amount,
                    CurrencyType.NGN,
                    TransactionType.Credit,
                    $"Paystack Deposit"
                );

                // await _transactionRepository.AddAsync(recipientTransaction);

                var transferGLTransaction = new GlTransactions(
                    transferGl.Id,
                    userId.Result.Id,
                    recipientTransaction.TransactionId,
                    amount,
                    CurrencyType.NGN,
                    TransactionType.Credit,
                    TransactionStatus.Completed

                );

                var depositGLTransaction = new GlTransactions(
                    depositGL.Id,
                    userId.Result.Id,
                    recipientTransaction.TransactionId,
                    -amount,
                    CurrencyType.NGN,
                    TransactionType.Credit,
                    TransactionStatus.Completed

                );
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailedResponse(null, $"Failed to deposit transaction: {ex.Message}");
            }

            await _transactionsRepository.UpdateStatusAsync(transaction.TransactionId, TransactionStatus.Completed);

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Webhook processed successfully");

        }

        catch (Exception ex)
        {
            return ApiResponse<string>.FailedResponse($"Failed to handle webhook: {ex.Message}");
        }


    }
    }