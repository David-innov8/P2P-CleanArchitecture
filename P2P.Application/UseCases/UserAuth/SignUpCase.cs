using System.ComponentModel.DataAnnotations;
using System.Transactions;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.EmailService;
using P2P.Application.Validators;
using P2P.Domains;
using P2P.Domains.Entities;
using P2P.Domains.ValueObjects;

namespace P2P.Application.UseCases;

public class SignUpCase : IRegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SignUpValidator _validator;
    private readonly IAccountNumberGenerator _accountNumberGenerator;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmtpService _smtpService;
    private readonly IEmailOutboxService _emailOutboxService;

    public SignUpCase(IUserRepository userRepository, IPasswordHasher passwordHasher, SignUpValidator validator,
        IAccountNumberGenerator accountNumberGenerator, IAccountRepository accountRepository, IUnitOfWork unitOfWork, ISmtpService smtpService, IEmailOutboxService emailOutboxService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _accountNumberGenerator = accountNumberGenerator;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _smtpService = smtpService;
        _emailOutboxService = emailOutboxService;
    }


      public async Task<ApiResponse<string>> UserSignUp(SignUpDto signUpDto)
    {  
        FluentValidation.Results.ValidationResult validationResult = _validator.Validate(signUpDto);
                     if (!validationResult.IsValid)
                     {
                         // Combine all validation error messages into a single string
                         var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                         return ApiResponse<string>.FailedResponse(errorMessage, "Validation Failed");
                     }
                     
                     var existingUser =  _userRepository.Query().FirstOrDefault(u=> u.Email == signUpDto.Email || u.Username == signUpDto.Username);
                
                     if (existingUser != null)
                     {
                         if (existingUser.Username == signUpDto.Username)
                         {
                             return ApiResponse<string>.FailedResponse("Username is already taken.", "Duplicate Username");
                         }
                         else
                         {
                             return ApiResponse<string>.FailedResponse("User with this email already exists.", "Duplicate Email");
                         }
                     }
                     
                     byte[] passwordSalt, passwordHash;
                     _passwordHasher.HashPassword(signUpDto.Password, out passwordSalt, out passwordHash);

                     var passwordValueObject = new PasswordHash(passwordHash, passwordSalt);
                
                     
                     var newUser = new User(
                                             signUpDto.Username, 
                                             signUpDto.Email, 
                                             signUpDto.PhoneNumber, 
                                             passwordValueObject, 
                                             signUpDto.FirstName, 
                                             signUpDto.LastName, 
                                             signUpDto.Gender, 
                                             signUpDto.DOB
                                         );
                     
        
        try
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Use TransactionScope to ensure atomicity





                // Step 5: Add user to the repository
                await _userRepository.AddUserAsync(newUser);
                await _unitOfWork.SaveChangesAsync();
                // Step 6: Generate a unique account number
                string accountNumber;
                bool isAccountNumberUnique = false;

                do
                {
                    accountNumber = _accountNumberGenerator.GenerateAccNo();
                    var existingAccount = await _accountRepository.GetAccountByNumberAsync(accountNumber);
                    isAccountNumberUnique = existingAccount == null;
                } while (!isAccountNumberUnique);

                // Step 7: Create the account with default currency
                var defaultAccount = new Account(newUser.Id, accountNumber, CurrencyType.NGN);

                // Step 8: Add account to repository
                await _accountRepository.AddAsync(defaultAccount);
                await _unitOfWork.SaveChangesAsync();
                // Complete the transaction
                scope.Complete();
                
             
            }
            var placeholders = new Dictionary<string, string>
            {
                { "{UserName}", signUpDto.FirstName },
                { "{Action}", "Signing up" },
                { "{CallToActionURL}", "https://example.com/dashboard" },
                { "{UnsubscribeURL}", "https://example.com/unsubscribe" }
            };

            // Queue email asynchronously - this won't block user registration
            await _emailOutboxService.QueueEmailAsync(
                signUpDto.Email, 
                "Welcome to Our Platform!", 
                "WelcomeTemplate.html", 
                placeholders);
            return ApiResponse<string>.SuccessResponse("User successfully signed up.", "Registration Complete");
        }
        catch (Exception ex)
        {
            // Log the exception
            return ApiResponse<string>.FailedResponse($"An error occurred during registration: {ex.Message}", "Registration Failed");
        }
    }

}

