using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;

using System.Security.Claims;
using P2p_Clean_Architecture________b;

namespace P2P.Infrastructure.Services;

public class JwtTokenGenerator:IJwtTokenGenerator
{
    private readonly AppSettings  _appSettings;
   
    public JwtTokenGenerator(AppSettings appSettings)
    {
        _appSettings = appSettings;
        


    }

    public string GenerateUserJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };
            
        var token = new JwtSecurityToken(
            issuer: _appSettings.JwtIssuer,
            audience: _appSettings.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appSettings.JwtDurationMinutes),
            signingCredentials: cred
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateResetToken(string email)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            // new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(15).ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _appSettings.JwtIssuer,
            audience: _appSettings.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appSettings.JwtDurationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public bool ValidateToken(string email, string token)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtKey));
            var handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your-issuer",
                ValidAudience = "your-audience",
                IssuerSigningKey = securityKey
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            return emailClaim == email;
        }
        catch
        {
            return false;
        }
    }
    

}