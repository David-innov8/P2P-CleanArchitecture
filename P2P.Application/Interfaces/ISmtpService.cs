namespace P2P.Application.UseCases.Interfaces;

public interface ISmtpService
{
    Task SendEmail(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders);
}