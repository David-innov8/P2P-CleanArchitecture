using System.ComponentModel.DataAnnotations;
using System.Transactions;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
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

    public SignUpCase(IUserRepository userRepository, IPasswordHasher passwordHasher, SignUpValidator validator,
        IAccountNumberGenerator accountNumberGenerator, IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _accountNumberGenerator = accountNumberGenerator;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }


      public async Task<ApiResponse<string>> UserSignUp(SignUpDto signUpDto)
    {
        try
        {
            // Use TransactionScope to ensure atomicity
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Step 1: Validate the input
                FluentValidation.Results.ValidationResult validationResult = _validator.Validate(signUpDto);
                if (!validationResult.IsValid)
                {
                    // Combine all validation error messages into a single string
                    var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return ApiResponse<string>.FailedResponse(errorMessage, "Validation Failed");
                }

                // Step 2: Check if the user already exists
                var existingUser = await _userRepository.GetUserByEmailAsync(signUpDto.Email);
                
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

                var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(signUpDto.Username);
                if (existingUserByUsername != null)
                {
                    return ApiResponse<string>.FailedResponse("The username is already taken.", "Duplicate Username");
                }
                
                // Step 3: Hash the password
                byte[] passwordSalt, passwordHash;
                _passwordHasher.HashPassword(signUpDto.Password, out passwordSalt, out passwordHash);

                var passwordValueObject = new PasswordHash(passwordHash, passwordSalt);
                
                // Step 4: Create new user
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
                // await _unitOfWork.
                // Complete the transaction
                scope.Complete();
                
                return ApiResponse<string>.SuccessResponse("User successfully signed up.", "Registration Complete");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            return ApiResponse<string>.FailedResponse($"An error occurred during registration: {ex.Message}", "Registration Failed");
        }
    }

}

