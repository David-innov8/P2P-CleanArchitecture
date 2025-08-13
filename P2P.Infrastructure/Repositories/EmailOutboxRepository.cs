using Microsoft.EntityFrameworkCore;
using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class EmailOutboxRepository:Repository<EmailOutbox>,IEmailOutboxRepository
{
    private readonly IUnitOfWork _unitOfWork;
    
    public EmailOutboxRepository(P2pContext context, IUnitOfWork unitOfWork) : base(context)
    {
        
        _unitOfWork = unitOfWork;   
     
    }
    
    public async Task<List<EmailOutbox>> GetPendingEmailsAsync(int batchSize = 10)
    {
        return await _unitOfWork
            .GetRepository<EmailOutbox>()
            .Query()
            .Where(e => !e.IsProcessed &&
                        e.RetryCount < 3 &&
                        (!e.NextRetryAt.HasValue || e.NextRetryAt <= DateTime.UtcNow))
            .OrderBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync();
    }
    
    public async Task<int> GetPendingCountAsync()
    {
        return await _unitOfWork
            .GetRepository<EmailOutbox>()
            .Query()
            .CountAsync(e => !e.IsProcessed &&
                             e.RetryCount < 3 &&
                             (!e.NextRetryAt.HasValue || e.NextRetryAt <= DateTime.UtcNow));
    }

    public async Task<List<EmailOutbox>> GetDeadLetterEmailsAsync()
    {
        return await _unitOfWork
            .GetRepository<EmailOutbox>()
            .Query()
            .Where(e => e.RetryCount >= 3 && !e.IsProcessed)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }
    
  
    
}