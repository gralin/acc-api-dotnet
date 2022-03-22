using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Gralin.Avigilon.ControlCenterAPI.Tests;

public class WebEndpointClientTests
{
    public const string UserNonce = "UserNonce";
    public const string UserKey = "UserKey";
    public const string ApiAddress = "https://127.0.0.1:8443/mt/api/rest/v1/";
    public const string Username = "Username";
    public const string Password = "Password";

    private readonly ILogger _logger;
    private readonly WebEndpointClientFactory _clientFactory;

    public WebEndpointClientTests()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter(typeof(WebEndpointClient).Namespace, LogLevel.Debug);
            builder.AddConsole();
        });

        _logger = loggerFactory.CreateLogger(typeof(WebEndpointClient));

        _clientFactory = new WebEndpointClientFactory(UserNonce, UserKey);
    }

    public HttpClient MockHttpClient(Action<IProtectedMock<HttpMessageHandler>> setupHandler)
    {
        var httpMessageHandler = new Mock<HttpMessageHandler>();
        setupHandler(httpMessageHandler.Protected());
        return new HttpClient(httpMessageHandler.Object);
    }

    [Test]
    public async Task should_login()
    {
        var httpClient = MockHttpClient(h => h
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@"
                {
                    ""status"": ""success"",
                    ""result"":
                    {
                        ""session"": ""session"",
                        ""externalUserId"": ""externalUserId"",
                        ""domainId"": ""domainId""
                    }
                }", Encoding.UTF8, "application/json")
            }));

        var client = _clientFactory.Create(new Uri(ApiAddress), httpClient, _logger);

        await client.Login(Username, Password);
    }
}