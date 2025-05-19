using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2P.Application.DTOs;
using P2P.Application.DTOs.AccountDto_Response;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Domains.Entities;
using P2P.Domains.Exceptions;

namespace P2p_Clean_Architecture________b.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AccountController : ControllerBase
{
    private readonly ITransferCase _transferCase;
    private readonly IGetUserAccountDetails _getUserAccountDetails; 
    private readonly ITransactionHistory _transactionHistory;
    public AccountController(ITransferCase transferCase, IGetUserAccountDetails getUserAccountDetails, ITransactionHistory transactionHistory)
    {
        _transferCase = transferCase;
        _getUserAccountDetails = getUserAccountDetails;
        _transactionHistory = transactionHistory;
    }



    [HttpPost("Transfer")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Transfer  ([FromBody] TransferRequestDto  request)
    {
        try
        {
            var result = await _transferCase.ExecuteTransfer(
                recipientUsername: request.RecipientUsername,
                amount: request.Amount,
                currency: request.Currency);

            if (result.Success)
            {
                return Ok(result.Data); // Returns TransferDTO with transaction details
            }

            return BadRequest(new
            {
                Message = result.Message,
                Data = result.Data // Optional: include failed DTO or null
            });

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
            // Log the exception here in production
            return StatusCode(500, new { Message = "An internal error occurred.", Error = ex.Message });
        }
    }


    [HttpPost("GetUserAccounts")]
    [Authorize(Roles = "User")]

    public async Task<IActionResult> GetUserAccounts()
    {
        try
        {
            var response = await _getUserAccountDetails.GetUserAccountInfo();

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
    
    [HttpGet("TransactionsHistory")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetTransactionHistory(
        [FromQuery] GetTransactionHistoryRequest request)
    {
        var result = await _transactionHistory.GetTransactionsByUserIdAsync(
            request.PageNumber,
            request.PageSize);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }
}