namespace P2P.Application.UseCases.Interfaces;

public interface IOtpService
{
    string GenerateOtp();
    void StoreOtp(Guid userId, string otp);
    bool VerifyOtp(Guid userId, string otp);

    void RemoveOtp(Guid userId);

}