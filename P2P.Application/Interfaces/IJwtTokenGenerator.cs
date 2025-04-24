using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateUserJwtToken(User user);
}