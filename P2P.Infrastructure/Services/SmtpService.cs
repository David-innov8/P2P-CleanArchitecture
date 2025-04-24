using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using P2P.Application.UseCases.Interfaces;

namespace P2P.Infrastructure.Services;

public class SmtpService:ISmtpService
{
    private readonly SmtpSettings _smtpSettings;

    public SmtpService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }
    
    
       public async Task SendEmail(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders)
    {

        // Validate input parameters
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            throw new ArgumentException("Email address cannot be null or empty.", nameof(toEmail));
        }
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Subject cannot be null or empty.", nameof(subject));
        }
        if (string.IsNullOrWhiteSpace(templateName))
        {
            throw new ArgumentException("Template name cannot be null or empty.", nameof(templateName));
        }

        // Validate SMTP settings
        if (string.IsNullOrWhiteSpace(_smtpSettings.Username))
        {
            throw new InvalidOperationException("SMTP username is not configured.");
        }
        if (string.IsNullOrWhiteSpace(_smtpSettings.Server))
        {
            throw new InvalidOperationException("SMTP server is not configured.");
        }
        // Log the email details for debugging
        Console.WriteLine($"Sending email to: {toEmail}, Subject: {subject}");



        try
        {

            using (var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                smtpClient.EnableSsl = _smtpSettings.EnableSsl;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_smtpSettings.Username);
                    mailMessage.Sender = new MailAddress(_smtpSettings.Username);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    string body = LoadTemplate(templateName, placeholders);
                    // string body = GetHardcodedTemplate(placeholders);
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;


                    Console.WriteLine($"Attempting to send email to: {toEmail}, with subject: {subject}");


                    await smtpClient.SendMailAsync(mailMessage);

                    Console.WriteLine($"Email sent successfully to: {toEmail}, with subject Subject: {subject}");
                }







            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            throw;
        }

    }
       
    private string LoadTemplate(string templateName, Dictionary<string, string> placeholders)
    {
        // Get the application's base directory
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", ".."));

        Console.WriteLine($"Base Directory: {baseDirectory}");
        Console.WriteLine($"project root Directory: {projectRoot}");
        // Construct the template path
        var templatePath = Path.Combine(projectRoot, "P2P.Infrastructure", "HtmlTemplates", templateName);

        // Log the constructed path for debugging
        Console.WriteLine($"Looking for template at: {templatePath}");

        // Check if the file exists
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var template = File.ReadAllText(templatePath);

        foreach (var placeholder in placeholders)
        {
            template = template.Replace(placeholder.Key, placeholder.Value);
        }

        return template;
    }


}