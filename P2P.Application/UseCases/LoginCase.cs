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

    public LoginCase( IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
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

        return ApiResponse<LoginResponse>.SuccessResponse(new LoginResponse()
        {
            Token = token,
            Username = existingUser.Username,
            Email = existingUser.Email,
            ProfilePicture = existingUser.Profile.ProfileImage
            
        });
    }
}