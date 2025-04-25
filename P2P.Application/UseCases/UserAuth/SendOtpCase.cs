using P2P.Application.UseCases.Interfaces;

namespace P2P.Application.UseCases;

public class SendOtpCase:ISendOtpCase
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private ISmtpService _smtpService;
    private ISetPinCase _setPin;
    public SendOtpCase(IUserRepository userRepository, IOtpService otpService, ISmtpService smtpService, ISetPinCase setPin)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _smtpService = smtpService;
        _setPin = setPin;
    }
    public async Task Handle()
    {

        var user = await _userRepository.GetUserFromClaimsAsync();
        if (user == null)
            throw new InvalidOperationException("User not found.");

        // Generate OTP
        string otp = _otpService.GenerateOtp();

        try
        {
      
            _otpService.StoreOtp(user.Id, otp);

   
            var placeholders = new Dictionary<string, string>
            {
                { "{OTPCode}", otp }
            };
        
      
            await _smtpService.SendEmail(user.Email, "Set Pin Otp", "OTPTemplate.html", placeholders);
        }
        catch (Exception ex)
        {
         
        
            // Remove the OTP from cache if email fails
            _otpService.RemoveOtp(user.Id);
        
        
            throw;
        }
    }

    public async Task Verify(string otp, string newPin )
    {
        var user = await _userRepository.GetUserFromClaimsAsync();
        _otpService.VerifyOtp(user.Id, otp);
        _setPin.Handle(newPin);
    }
}