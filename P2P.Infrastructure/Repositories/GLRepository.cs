using Microsoft.EntityFrameworkCore;
using P2P.Application.Interfaces.Repositories;
using P2P.Domains;
using P2P.Domains.Entities;
using P2P.Infrastructure.Context;

namespace P2P.Infrastructure.Repositories;

public class GLRepository:Repository<GeneralLedger>, IGLRepository
{
    public GLRepository(P2pContext context) : base(context)
    {
    }

    public async Task<GeneralLedger> GetByNameAndCurrencyAsync(string name, CurrencyType currency)
    {
        return await _context.Set<GeneralLedger>()
            .FirstOrDefaultAsync(gl => gl.AccountName == name && gl.Currency == currency);
    }

    public async Task<GeneralLedger> GetSystemGLByTypeAndCurrencyAsync(GLType type, CurrencyType currency)
    {
        string glNamePrefix = type.ToString();
        string glName = $"{currency.ToString()}{glNamePrefix}";
        
        return await _context.Set<GeneralLedger>()
            .FirstOrDefaultAsync(gl => gl.AccountName == glName && gl.Type == type && gl.Currency == currency);
    }

    public async Task<IEnumerable<GeneralLedger>> GetAllByTypeAsync(GLType type)
    {
        return await _context.Set<GeneralLedger>()
            .Where(gl => gl.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<GeneralLedger>> GetAllByCurrencyAsync(CurrencyType currency)
    {
        return await _context.Set<GeneralLedger>()
            .Where(gl => gl.Currency == currency)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string name, CurrencyType currency)
    {
        return await _context.Set<GeneralLedger>()
            .AnyAsync(gl => gl.AccountName == name && gl.Currency == currency);
    }
}