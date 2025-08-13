namespace P2P.Application.UseCases.Interfaces;

public interface IAccountNumberGenerator
{
    Task<string> GenerateAccNoAsync();
}