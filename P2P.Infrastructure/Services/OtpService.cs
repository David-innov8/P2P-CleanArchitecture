using Microsoft.Extensions.Caching.Memory;
using P2P.Application.UseCases.Interfaces;
using StackExchange.Redis;

namespace P2P.Infrastructure.Services;

public class OtpService:IOtpService
{
  private readonly IDatabase _redisDb;
  private const int OtpLength = 6;

  public OtpService(IConnectionMultiplexer redis)
  {
    _redisDb = redis.GetDatabase();
    

  }

  
  public string GenerateOtp()
  {
    Random random = new Random();
    return random.Next((int)Math.Pow(10, OtpLength - 1), (int)Math.Pow(10, OtpLength)).ToString();
  }

  private string GetCacheKey(Guid userId)
  {
    return $"OTP:{userId}";
  }


  public void StoreOtp(Guid userId, string otp)
  {
    var cacheKey = GetCacheKey(userId);
    _redisDb.StringSet(cacheKey, otp, TimeSpan.FromMinutes(5));
  }

  public bool VerifyOtp(Guid userId, string otp)
  {
    var cacheKey = GetCacheKey(userId);
    var storedOtp = _redisDb.StringGet(cacheKey);
    if (!string.IsNullOrEmpty(storedOtp) && storedOtp == otp)
    {
      _redisDb.KeyDelete(cacheKey); // Invalidate OTP after successful verification
      return true;
    }
    return false;
  }

  public void RemoveOtp(Guid userId)
  {
    var cacheKey = GetCacheKey(userId);
    _redisDb.KeyDelete(cacheKey);
  }
  
  
}