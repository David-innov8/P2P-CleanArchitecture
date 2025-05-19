namespace P2P.Application.UseCases.Interfaces.GeneralLedgers;

public interface IInitializeGlCase
{
    Task<bool> InitializeSystemGLs();
}