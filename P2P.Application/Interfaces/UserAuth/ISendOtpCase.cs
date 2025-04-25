namespace P2P.Application.UseCases.Interfaces;

public interface ISendOtpCase
{
     Task Handle();
     Task Verify(string otp, string newPin);

}