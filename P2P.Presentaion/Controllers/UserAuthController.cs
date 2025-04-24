using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;

namespace P2p_Clean_Architecture________b.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserAuthController : ControllerBase
{
    private readonly IRegisterUserUseCase _registerUserUseCase;
    private readonly ILoginUserUseCase _loginUserUseCase;
    private readonly IUpdatePasswordUseCase _updatePasswordUseCase;
    
    public UserAuthController(IRegisterUserUseCase registerUserUseCase, ILoginUserUseCase loginUserUseCase, IUpdatePasswordUseCase updatePasswordUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUserUseCase = loginUserUseCase;
        _updatePasswordUseCase = updatePasswordUseCase;
    }


    [HttpPost("Signup")]


    public async Task<ActionResult<ApiResponse<string>>> SignUp(SignUpDto request)
    {
        var response = await _registerUserUseCase.UserSignUp(request);

      
        
      return  Ok(response);
    }

    [HttpPost("Login")]

    public async Task<ActionResult<ApiResponse<string>>> Login(LoginDto request)
    {
        var response = await _loginUserUseCase.Login(request);
        return  Ok(response);
    }
    
    [HttpPost("UpdatePassword")]
    [Authorize (Roles = "User")]

    public async Task<ActionResult<ApiResponse<string>>> UpdatePassword(UpdatePasswordDTO request)
    {
        var response = await _updatePasswordUseCase.ExecuteAsync(request);
        return  Ok(response);
    }

}