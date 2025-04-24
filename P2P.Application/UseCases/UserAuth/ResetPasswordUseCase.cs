using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Domains.ValueObjects;

namespace P2P.Application.UseCases;

public class ResetPasswordUseCase:IResetPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordUseCase(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<string>> ResetPassword(ResetPasswordRequestDto request)
    {
        // Step 1: Validate input
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
        {
            return ApiResponse<string>.FailedResponse("All fields are required.", "Validation Failed");
        }

        // Step 2: Validate the token
        if (!_tokenGenerator.ValidateToken(request.Email, request.Token))
        {
            return ApiResponse<string>.FailedResponse("Invalid or expired token.", "Invalid Token");
        }

        // Step 3: Find the user by email
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return ApiResponse<string>.FailedResponse("User not found.", "Invalid Email");
        }
        
        byte[] passwordSalt, passwordHash;
        var hashedPassword = _passwordHasher.HashPassword(request.NewPassword, out passwordSalt, out passwordHash);

        var newPasswordValueObject = new PasswordHash(passwordHash, passwordSalt);

        // Step 5: Update the user's password
        user.UpdatePassword(newPasswordValueObject);
        await _userRepository.UpdateUserAsync(user);

        return ApiResponse<string>.SuccessResponse("Password reset successfully.", "Password Reset");

    }

}