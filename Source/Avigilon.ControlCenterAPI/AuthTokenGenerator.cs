using System.Security.Cryptography;
using System.Text;

namespace Gralin.Avigilon.ControlCenterAPI;

public class AuthTokenGenerator
{
    private readonly int _timeStamp;

    public string UserNonce { get; set; }
    public string UserKey { get; set; }

    public AuthTokenGenerator(string userNonce, string userKey, Func<DateTime> utcNowFunc = null)
    {
        UserNonce = userNonce;
        UserKey = userKey;

        var utcNow = utcNowFunc?.Invoke() ?? DateTime.UtcNow;
        _timeStamp = (int)utcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public string GenerateToken()
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(_timeStamp + UserKey));
        var hexEncoded = new StringBuilder(hash.Length * 2);
        foreach (var t in hash)
            hexEncoded.Append(t.ToString("x2"));
        var token = $"{UserNonce}:{_timeStamp}:{hexEncoded}";
        return token;
    }
}