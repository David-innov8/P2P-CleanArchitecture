using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2P.Application.UseCases.Interfaces.Paystack;

namespace P2p_Clean_Architecture________b.Controllers;


[Route("api/[controller]")]
[ApiController]

public class Paystack : ControllerBase
{
  
    private readonly IPaystackService _paystackService;

    public Paystack( IPaystackService paystackService)
    {
        _paystackService = paystackService;
    }


    [HttpPost("PaystackInitiate")]
    [Authorize(Roles = "User")]

    public async Task<IActionResult> Transfer(decimal amount)
    {
        var result = await _paystackService.InitializePayment(amount);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Data);
    }
    
    [HttpPost("Paystack-verifyPayment")]
    [Authorize(Roles = "User")]

    public async Task<IActionResult> VerifyPayment(string reference)
    {
        var result = await _paystackService.VerifyPayment(reference);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result.Data);
    }
    
}