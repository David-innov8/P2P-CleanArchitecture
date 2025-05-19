using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Domains.Entities;

namespace P2p_Clean_Architecture________b.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserAndTransactionAuthController : ControllerBase
{
    private readonly IRegisterUserUseCase _registerUserUseCase;
    private readonly ILoginUserUseCase _loginUserUseCase;
    private readonly IUpdatePasswordUseCase _updatePasswordUseCase;
    private readonly IResetPasswordUseCase _resetPasswordUseCase;
    private readonly IForgotPasswordCase _forgotPasswordUseCase;
    private readonly ISendOtpCase _sendOtp;
    
    public UserAndTransactionAuthController(IRegisterUserUseCase registerUserUseCase, ILoginUserUseCase loginUserUseCase, IUpdatePasswordUseCase updatePasswordUseCase, IResetPasswordUseCase resetPasswordUseCase, IForgotPasswordCase forgotPasswordUseCase, ISendOtpCase sendOtp)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUserUseCase = loginUserUseCase;
        _updatePasswordUseCase = updatePasswordUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _forgotPasswordUseCase = forgotPasswordUseCase;
        _sendOtp = sendOtp;
    }


    [HttpPost("Signup")]


    public async Task<ActionResult<ApiResponse<string>>> SignUp(SignUpDto request)
    {
        var response = await _registerUserUseCase.UserSignUp(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }
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
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var response = await _forgotPasswordUseCase.ForgotPassword(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var response = await _resetPasswordUseCase.ResetPassword(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
    
    [HttpPost("send-otp")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> SendOtp()
    {
        await _sendOtp.Handle();
        return Ok(new { Message = "OTP sent successfully." });
    }

    [HttpPost("ResetTransactionPin")]
    public async Task<IActionResult> ResetTransactionPin([FromBody] ResetPinDTO reset)
    {
        await _sendOtp.Verify(reset.otp, reset.newPin);
        return Ok(new { Message = "OTP Reset successfully." });
    }

}