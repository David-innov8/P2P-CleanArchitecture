using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases;

public class ForgetPasswordCase: IForgotPasswordCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ISmtpService _smtpService;

    public ForgetPasswordCase(IUserRepository userRepository,IPasswordHasher passwordHasher,IJwtTokenGenerator jwtTokenGenerator, ISmtpService smtpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _smtpService = smtpService;
    }

    public async Task<ApiResponse<string>> ForgotPassword(ForgotPasswordRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return ApiResponse<string>.FailedResponse("Email is required.", "Validation Failed");
        }
        
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return ApiResponse<string>.SuccessResponse(null ,"Pls Check your email for the reset link");
        }
        
        var token = _jwtTokenGenerator.GenerateResetToken(user.Email);
        
        var resetLink = GeneratePasswordResetLink(token, user.Email);
        var placeholders = new Dictionary<string, string>
        {
            { "{ResetLink}", resetLink },
            { "{UserName}", user.Email } // Assuming you want to use the email as the username
        };
         
        await _smtpService.SendEmail(user.Email, "Reset Password", "ForgotPasswordTemplate.html", placeholders);
        return ApiResponse<string>.SuccessResponse(token, "Email Sent");

    }
    
    private string GeneratePasswordResetLink(string token, string email)
    {
        // Move the base URL to appsettings.json for flexibility
        var baseUrl = "http://localhost:4200/resetpassword";

        var resetUrl = $"{baseUrl}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
        return resetUrl;

    }
}