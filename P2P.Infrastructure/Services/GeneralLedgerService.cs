using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.GeneralLedgers;
using P2P.Domains;
using P2P.Domains.Entities;

namespace P2P.Infrastructure.Services;

public class GeneralLedgerService:IGLService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGLRepository _glRepository;

    public GeneralLedgerService(IUnitOfWork unitOfWork, IGLRepository glRepository)
    {
        _glRepository = glRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<GeneralLedger> CreateGL(string accountName, CurrencyType currency, string description, GLType type, bool canRunNegative = false, decimal minimumBalance = 0m)
    {
        // Check if GL already exists
        bool exists = await _glRepository.ExistsAsync(accountName, currency);
        if (exists)
        {
            throw new InvalidOperationException($"GL with name {accountName} for currency {currency} already exists");
        }

        var gl = new GeneralLedger(accountName, currency, description, type, canRunNegative, minimumBalance);
        await _glRepository.AddAsync(gl);
        await _unitOfWork.SaveChangesAsync();
        return gl;
    }
    
    public async Task<bool> InitializeSystemGLs()
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Create all necessary system GLs for each currency
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                // Create Transfer GL (can't go negative)
                string transferGLName = $"{currency.ToString()}TransferGL";
                if (!await _glRepository.ExistsAsync(transferGLName, currency))
                {
                    await CreateGL(
                        transferGLName,
                        currency,
                        $"System GL for managing {currency.ToString()} transfers",
                        GLType.TransferGL,
                        false,
                        0m);
                }

                // Create Deposit GL (can go negative)
                string depositGLName = $"{currency.ToString()}DepositGL";
                if (!await _glRepository.ExistsAsync(depositGLName, currency))
                {
                    await CreateGL(
                        depositGLName,
                        currency,
                        $"System GL for managing {currency.ToString()} deposits",
                        GLType.DepositGL,
                        true, // Can run negative
                        0m);
                }

                // Create Fee GL
                string feeGLName = $"{currency.ToString()}FeeGL";
                if (!await _glRepository.ExistsAsync(feeGLName, currency))
                {
                    await CreateGL(
                        feeGLName,
                        currency,
                        $"System GL for managing {currency.ToString()} fees",
                        GLType.FeeGL,
                        false,
                        0m);
                }
            }

            await _unitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<GeneralLedger> GetGLByTypeAndCurrency(GLType type, CurrencyType currency)
    {
        return await _glRepository.GetSystemGLByTypeAndCurrencyAsync(type, currency);
    }
}