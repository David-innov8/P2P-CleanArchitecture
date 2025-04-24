using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Domains.ValueObjects;

namespace P2P.Application.UseCases;

public class UpdatePasswordCase:IUpdatePasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISmtpService _smtpService;

    public UpdatePasswordCase(IUserRepository userRepository, IPasswordHasher passwordHasher, ISmtpService smtpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _smtpService = smtpService;
    }


    public async Task<ApiResponse<string>> ExecuteAsync( UpdatePasswordDTO updatePasswordDto)
    {
       
        if (string.IsNullOrEmpty(updatePasswordDto.OldPassword) || string.IsNullOrEmpty(updatePasswordDto.NewPassword))
        {
            return ApiResponse<string>.FailedResponse("Old and new passwords are required.", "Validation Failed");
        }
        
        // Step 2: Find the user by ID
        var user = await _userRepository.GetUserFromClaimsAsync();
        if (user == null)
        {
            return ApiResponse<string>.FailedResponse("User not found.", "Invalid User");
        }

        // Step 3: Check if the account is locked
        if (user.State.IsAccountLocked)
        {
            return ApiResponse<string>.FailedResponse("Account is locked. Please contact support.", "Account Locked");
        }
        
        if (!_passwordHasher.VerifyPassword(updatePasswordDto.OldPassword, user.Password.Salt, user.Password.Hash))
        {
            user.State.IncrementRetryCount();
            await _userRepository.UpdateUserAsync(user); // Save retry count increment

            return ApiResponse<string>.FailedResponse("Incorrect old password.", "Invalid Credentials");
        }
        
        // Step 5: Hash the new password
        byte[] newPasswordSalt, newPasswordHash;
        _passwordHasher.HashPassword(updatePasswordDto.NewPassword, out newPasswordSalt, out newPasswordHash);
        var newPasswordValueObject = new PasswordHash( newPasswordHash, newPasswordSalt);

        // Step 6: Update the password
        user.UpdatePassword(newPasswordValueObject);
        await _userRepository.UpdateUserAsync(user);

        // Step 8: Return success response
        return ApiResponse<string>.SuccessResponse(null, "Password Updated");
    }
}