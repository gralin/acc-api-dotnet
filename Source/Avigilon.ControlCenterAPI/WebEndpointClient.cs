using System.Text;
using System.Text.Json;
using Gralin.Avigilon.ControlCenterAPI.DataContracts;
using Microsoft.Extensions.Logging;

namespace Gralin.Avigilon.ControlCenterAPI;

public class WebEndpointClient : IDisposable
{
    private readonly ILogger _logger;
    private readonly AuthTokenGenerator _tokenGenerator;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerSettings;
    private Uri _baseUri;
    private string _session;

    private WebEndpointClient(string userNonce, string userKey, ILogger logger = null)
    {
        _tokenGenerator = new AuthTokenGenerator(userNonce, userKey);
        _logger = logger;
        _serializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public WebEndpointClient(string userNonce, string userKey, HttpClient httpClient, ILogger logger = null)
        : this(userNonce, userKey, logger)
    {
        _httpClient = httpClient;
    }

    public WebEndpointClient(string userNonce, string userKey, bool ignoreSslErrors = true, ILogger logger = null)
        : this(userNonce, userKey, logger)
    {
        var httpClientHandler = new HttpClientHandler();

        if (ignoreSslErrors)
            httpClientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        _httpClient = new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromSeconds(10) };
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    public void Setup(string baseUri)
    {
        _baseUri = new Uri(baseUri);
    }

    public async Task Login(string username, string password, string clientName = nameof(WebEndpointClient))
    {
        try
        {
            var requestUri = new Uri(_baseUri, "login");
            var loginRequest = new LoginRequestContract
            {
                AuthorizationToken = _tokenGenerator.GenerateToken(),
                Username = username,
                Password = password,
                ClientName = clientName
            };

            var body = JsonSerializer.Serialize(loginRequest, _serializerSettings);

            _logger?.Log(LogLevel.Debug, $"-> POST {requestUri}");
            var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, requestContent);
            _logger?.Log(LogLevel.Debug, $"<- {response.StatusCode} ({requestUri})");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to login: response status code was {response.StatusCode}");

            var responseStream = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponseContract>(
                responseStream, _serializerSettings);

            if (loginResponse?.Status != "success")
                throw new Exception($"Failed to login: {loginResponse?.Status}");

            _session = loginResponse.Result.Session;
            _logger?.Log(LogLevel.Information, "Successfully logged into WebEndpoint Service");
        }
        catch (Exception e)
        {
            _logger?.Log(LogLevel.Warning, "Failed to login into WebEndpoint Service", e);
            throw;
        }
    }

    public async Task<List<CameraContract>> GetCameras()
    {
        try
        {
            var requestUri = new Uri(_baseUri, $"cameras?verbosity=LOW&session={_session}");

            _logger?.Log(LogLevel.Debug, $"-> GET {requestUri.GetLeftPart(UriPartial.Path)}");
            var response = await _httpClient.GetAsync(requestUri);
            _logger?.Log(LogLevel.Debug, $"<- {response.StatusCode} ({requestUri.GetLeftPart(UriPartial.Path)})");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get cameras: response status code was {response.StatusCode}");

            var responseStream = await response.Content.ReadAsStreamAsync();
            var camerasResponse = await JsonSerializer.DeserializeAsync<CameraResponseContract>(
                responseStream, _serializerSettings);

            if (camerasResponse?.Status != "success")
                throw new Exception($"Failed to get cameras: {camerasResponse?.Status}");

            return camerasResponse.Result.Cameras;
        }
        catch (Exception e)
        {
            _logger?.Log(LogLevel.Warning, "Failed to get cameras from WebEndpoint Service", e);
            throw;
        }
    }

    public async Task Logout()
    {
        try
        {
            var requestUri = new Uri(_baseUri, "logout");
            var body = JsonSerializer.Serialize(new { session = _session }, _serializerSettings);

            _logger?.Log(LogLevel.Debug, $"-> POST {requestUri}");
            var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, requestContent);
            _logger?.Log(LogLevel.Debug, $"<- {response.StatusCode} ({requestUri})");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to logout: response status code was {response.StatusCode}");

            _session = null;
            _logger?.Log(LogLevel.Information, "Successfully logged out from WebEndpoint Service");
        }
        catch (Exception e)
        {
            _logger?.Log(LogLevel.Warning, "Failed to logout from into WebEndpoint Service", e);
            throw;
        }
    }
}