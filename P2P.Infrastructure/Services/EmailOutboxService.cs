using Microsoft.Extensions.Logging;
using P2P.Application.DTOs;
using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.EmailService;
using P2P.Domains.Entities;

namespace P2P.Infrastructure.Services;

public class EmailOutboxService:IEmailOutboxService
{
    private readonly IEmailOutboxRepository _emailOutboxRepository;
    private readonly ISmtpService _smtpService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmailOutboxService> _logger;

    public EmailOutboxService(
        IEmailOutboxRepository emailOutboxRepository,
        ISmtpService smtpService,
        IUnitOfWork unitOfWork,
        ILogger<EmailOutboxService> logger)
    {
        _emailOutboxRepository = emailOutboxRepository;
        _smtpService = smtpService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
     public async Task QueueEmailAsync(string to, string subject, string templateName, Dictionary<string, string> placeholders)
        {
            var emailOutbox = new EmailOutbox(to, subject, templateName, placeholders);
            await _emailOutboxRepository.AddAsync(emailOutbox);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Email queued for {To} with subject: {Subject}", to, subject);
        }

        public async Task<int> ProcessPendingEmailsAsync(int batchSize = 10)
        {
            var pendingEmails = await _emailOutboxRepository.GetPendingEmailsAsync(batchSize);
            
            if (!pendingEmails.Any())
                return 0;

            var processedCount = 0;
            
            foreach (var email in pendingEmails)
            {
                try
                {
                    var placeholders = email.GetPlaceholders();
                    
                    await _smtpService.SendEmail(email.To, email.Subject, email.TemplateName, placeholders);
                    
                    email.MarkAsProcessed();
                    processedCount++;
                    
                    _logger.LogInformation("Email sent successfully to {To}", email.To);
                }
                catch (Exception ex)
                {
                    email.MarkAsFailed(ex.Message);
                    _logger.LogError(ex, "Failed to send email to {To}, retry count: {RetryCount}", 
                        email.To, email.RetryCount);
                }
            }

            await _emailOutboxRepository.UpdateRangeAsync(pendingEmails);
            await _unitOfWork.SaveChangesAsync();

            return processedCount;
        }

        public async Task<EmailOutboxStats> GetStatsAsync()
        {
            // Implementation would query repository for various counts
            var pendingCount = await _emailOutboxRepository.GetPendingCountAsync();
            var deadLetterEmails = await _emailOutboxRepository.GetDeadLetterEmailsAsync();
            
            return new EmailOutboxStats
            {
                PendingCount = pendingCount,
                DeadLetterCount = deadLetterEmails.Count,
                // Add other stats as needed
            };
        }
}