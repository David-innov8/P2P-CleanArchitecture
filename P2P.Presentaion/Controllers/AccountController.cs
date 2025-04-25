using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2P.Application.DTOs;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains.Exceptions;

namespace P2p_Clean_Architecture________b.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AccountController : ControllerBase
{
    private readonly ITransferCase _transferCase;

    public AccountController(ITransferCase transferCase)
    {
        _transferCase = transferCase;
    }



    [HttpPost("Transfer")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Transfer  ([FromBody] TransferRequestDto  request)
    {
        try
        {
            await _transferCase.ExecuteTransfer(request.RecipientUsername, request.Amount, request.Currency);
            return Ok(new { Message = "Transfer completed successfully." });
        }
        catch (UserDoesntExistException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (AccountNotFoundException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InsufficientFundsException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { Message = "An internal error occurred." });
        }
    }
    
}