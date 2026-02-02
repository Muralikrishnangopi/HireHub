using Microsoft.Extensions.Caching.Memory;

public class OtpService
{
    private readonly IMemoryCache _cache;

    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void StoreOtp(string key, string otp)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        _cache.Set(key, otp, options);
    }

    public bool ValidateOtp(string key, string otp)
    {
        if (_cache.TryGetValue(key, out string? storedOtp))
        {
            if (storedOtp !=null && storedOtp == otp)
            {
                _cache.Remove(key); // OTP can be used only once
                return true;
            }
        }
        return false;
    }

    public void RemoveOtp(string key)
    {
        _cache.Remove(key);
    }
}

