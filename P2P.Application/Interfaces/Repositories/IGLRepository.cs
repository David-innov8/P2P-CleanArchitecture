using P2P.Application.UseCases.Interfaces;
using P2P.Domains;
using P2P.Domains.Entities;

namespace P2P.Application.Interfaces.Repositories;

public interface IGLRepository: IRepository<GeneralLedger>
{
    Task<GeneralLedger> GetByNameAndCurrencyAsync(string name, CurrencyType currency);

    Task<GeneralLedger> GetSystemGLByTypeAndCurrencyAsync(GLType type, CurrencyType currency);

    Task<IEnumerable<GeneralLedger>> GetAllByTypeAsync(GLType type);

    Task<IEnumerable<GeneralLedger>> GetAllByCurrencyAsync(CurrencyType currency);

    Task<bool> ExistsAsync(string name, CurrencyType currency);
}