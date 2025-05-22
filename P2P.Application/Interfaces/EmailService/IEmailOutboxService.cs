using P2P.Application.DTOs;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.EmailService;

public interface IEmailOutboxService
{

  
    
    Task QueueEmailAsync(string to, string subject, string templateName, Dictionary<string, string> placeholders);
    Task<int> ProcessPendingEmailsAsync(int batchSize = 10);
    Task<EmailOutboxStats> GetStatsAsync();
}