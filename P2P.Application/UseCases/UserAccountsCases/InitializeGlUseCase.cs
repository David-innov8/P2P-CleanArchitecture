using P2P.Application.UseCases.Interfaces.GeneralLedgers;

namespace P2P.Application.UseCases;

public class InitializeGlUseCase: IInitializeGlCase
{
    private readonly IGLService _glService;

    public InitializeGlUseCase(IGLService glService)
    {
        _glService = glService;
    }

    public async Task<bool> InitializeSystemGLs()
    {
        return await _glService.InitializeSystemGLs();
    }
}