using P2P.Domains;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.GeneralLedgers;

public interface IGLService
{
    Task<GeneralLedger> CreateGL(string accountName, CurrencyType currency, string description, GLType type, bool canRunNegative = false, decimal minimumBalance = 0m);
    Task<bool> InitializeSystemGLs();
    Task<GeneralLedger> GetGLByTypeAndCurrency(GLType type, CurrencyType currency);
}