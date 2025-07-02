using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace MoviesAPI.SDK.Consumer;

public class AuthTokenProvider
{
    private readonly HttpClient _httpClient;
    private string _cachedToken = string.Empty;
    private static readonly SemaphoreSlim Lock = new (1, 1); // ensures only one request can be processed at a time when a token is being generated

    public AuthTokenProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            JwtSecurityToken? jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);
            
            string expiration = jwt.Claims.Single(claim => claim.Type == "exp").Value;
            
            DateTime expiryDateTime = UnixTimeStampToDateTime(int.Parse(expiration));

            if (expiryDateTime > DateTime.UtcNow)
            {
                return _cachedToken;
            }
        }
        
        await Lock.WaitAsync();
        
        var requestBody = new
        {
            userid = "3a37dd07-723a-408c-9985-4bc20d07c338",
            email = "eviecox95@gmail.com",
            customClaims = new Dictionary<string, object>
            {
                { "admin", true },
                { "trusted_member", true }
            }
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", requestBody);
        
        string newToken = await response.Content.ReadAsStringAsync();
        _cachedToken = newToken;
        Lock.Release();
        return newToken;
    }

    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}