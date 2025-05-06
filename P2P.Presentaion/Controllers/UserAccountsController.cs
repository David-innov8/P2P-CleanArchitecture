using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2P.Application.DTOs.USerDTOs;
using P2P.Application.UseCases.Interfaces.UserAccounts;
using P2P.Domains.Entities;

namespace P2p_Clean_Architecture________b.Controllers;
 

[Route("api/[controller]")]
[ApiController]

public class UserAccountsController : Controller
{
    public readonly IUpdateUserUseCase _updateUserUseCase;
    public readonly IGetReciepientDetailsUSeCase _getReciepientDetailsUSeCase;
    public readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccountsController(IUpdateUserUseCase updateUserUseCase, IHttpContextAccessor httpContextAccessor, IGetReciepientDetailsUSeCase getReciepientDetailsUSeCase)
    {
        _updateUserUseCase = updateUserUseCase;
        _httpContextAccessor = httpContextAccessor;
        _getReciepientDetailsUSeCase = getReciepientDetailsUSeCase;
    }
    
    [HttpPost("UpdateUser")]
    [Authorize(Roles = "User")]
    
    public async Task<ActionResult<ApiResponse<UpdateUserDTO>>> EditUserDetails(UpdateUserDTO dto)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest(ApiResponse<UpdateUserDTO>.FailedResponse(null, "User ID not found in token"));
            }

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(ApiResponse<UpdateUserDTO>.FailedResponse(null, "Invalid user ID format"));
            }
            var response = await _updateUserUseCase.ExecuteAsync( dto,userId);

            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500,
                ApiResponse<string>.FailedResponse(null, "An error occurred while retrieving account information"));

        }
    }
    
    [HttpGet("GetReciepeintDetails{input}")]
    public async Task<IActionResult> GetRecipientDetails(string input)
    {
        var response = await _getReciepientDetailsUSeCase.ExecuteAsync(input);
    
        if (!response.Success)
        {
            return NotFound(response);
        }
    
        return Ok(response);
    }
}