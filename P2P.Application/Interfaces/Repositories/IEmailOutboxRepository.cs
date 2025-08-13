using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;

namespace P2P.Application.Interfaces.Repositories;

public interface IEmailOutboxRepository:IRepository<EmailOutbox>
{
    Task<List<EmailOutbox>> GetPendingEmailsAsync(int batchSize = 10);
 
    Task<int> GetPendingCountAsync();
    Task<List<EmailOutbox>> GetDeadLetterEmailsAsync();
}