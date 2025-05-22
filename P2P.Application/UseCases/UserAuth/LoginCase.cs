using Microsoft.AspNetCore.Http;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;
using P2P.Domains.Exceptions;

namespace P2P.Application.UseCases;

public class LoginCase: ILoginUserUseCase
{
    
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISmtpService _smtpService;

    public LoginCase( IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IHttpContextAccessor httpContextAccessor, ISmtpService smtpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _httpContextAccessor = httpContextAccessor;
        _smtpService = smtpService;
    }

    public async Task<ApiResponse<LoginResponse>> Login(LoginDto loginDto)
    {
        
        if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
        {
            return ApiResponse<LoginResponse>.FailedResponse(null, "Validation Failed");
        }
        var existingUser = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

        if (existingUser == null)
        {
            throw new UserDoesntExistException("Invalid username or password.");
        }
        
        if (!_passwordHasher.VerifyPassword(loginDto.Password, existingUser.Password.Salt, existingUser.Password.Hash))
        {
            return ApiResponse<LoginResponse>.FailedResponse(null, "Invalid Credentials");
        }

        var token = _jwtTokenGenerator.GenerateUserJwtToken(existingUser);

        
        var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown Device";
        
        var IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        
        // var location = await GetLocationFromIP(IpAddress);
        
        
        // var placeholders = new Dictionary<string, string>
        // {
        //     { "{UserName}", existingUser.Username },
        //     { "{DeviceName}", userAgent },
        //     {"{Location}", location},
        //     {"{LoginDateTime}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}
        //
        // };
        // await _smtpService.SendEmail(existingUser.Email, "Welcome Back!", "LoginTemplate.html", placeholders);

        return ApiResponse<LoginResponse>.SuccessResponse(new LoginResponse()
        {
            Token = token,
            Username = existingUser.Username,
            Email = existingUser.Email,
            ProfilePicture = existingUser.Profile.ProfileImage
            
        });
    }
    
    private async Task<string> GetLocationFromIP(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "::1") // Handle localhost
            return "Localhost";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}");
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic locationData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            return $"{locationData.city}, {locationData.country}";
        }

        return "Unknown Location";
    }

}