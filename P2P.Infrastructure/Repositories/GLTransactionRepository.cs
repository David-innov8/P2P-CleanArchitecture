using Microsoft.EntityFrameworkCore;
using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.GeneralLedgers;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class GLTransactionRepository: Repository<GlTransactions>, IGLTransactionRepository
{
   
    private readonly P2pContext _context;
 
   
    public GLTransactionRepository(P2pContext context, IUnitOfWork unitOfWork):base(context)
    {
    }
    
    public async Task<GlTransactions> GetByReferenceAsync(string reference)
    {
        return await _context.Set<GlTransactions>()
            .FirstOrDefaultAsync(t => t.PaystackReference == reference);
    }


}