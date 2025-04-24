using System.ComponentModel.DataAnnotations;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.Validators;
using P2P.Domains.Entities;
using P2P.Domains.ValueObjects;

namespace P2P.Application.UseCases;

public class SignUpCase: IRegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SignUpValidator _validator;
    public SignUpCase(IUserRepository userRepository, IPasswordHasher passwordHasher, SignUpValidator validator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<ApiResponse<string>> UserSignUp(SignUpDto signUpDto)
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
                throw new InvalidOperationException("Username is already taken.");
            }
            else
            {
                throw new InvalidOperationException("User with this email already exists.");
            }
        }

        var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(signUpDto.Username);
        if (existingUserByUsername != null)
        {
            return ApiResponse<string>.FailedResponse("The username is already taken.", "Duplicate Username");
        }
        byte[] passwordSalt, passwordHash;
        var hashedPassword = _passwordHasher.HashPassword(signUpDto.Password, out passwordSalt, out passwordHash);

        var passwordValueObject = new PasswordHash(  passwordHash, passwordSalt);
        
        var newUser = new User(signUpDto.Username, signUpDto.Email, signUpDto.PhoneNumber, passwordValueObject, signUpDto.FirstName, signUpDto.LastName, signUpDto.Gender, signUpDto.DOB);

        await _userRepository.AddUserAsync(newUser);

        return ApiResponse<string>.SuccessResponse("User successfully signed up.", "Registeration Complete");

    }
}