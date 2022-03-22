using Microsoft.Extensions.Logging;

namespace Gralin.Avigilon.ControlCenterAPI;

public class WebEndpointClientFactory
{
    private readonly string _userNonce;
    private readonly string _userKey;

    public WebEndpointClientFactory(string userNonce, string userKey)
    {
        _userNonce = userNonce;
        _userKey = userKey;
    }

    public WebEndpointClient Create(Uri address, bool ignoreSslErrors = true, ILogger logger = null)
        => new(_userNonce, _userKey, new Uri(address, "/mt/api/rest/v1/"), ignoreSslErrors, logger);

    public WebEndpointClient Create(Uri address, HttpClient httpClient, ILogger logger = null)
        => new(_userNonce, _userKey, new Uri(address, "/mt/api/rest/v1/"), httpClient, logger);
}