using System;
using FluentAssertions;
using NUnit.Framework;

namespace Gralin.Avigilon.ControlCenterAPI.Tests;

public class AuthTokenGeneratorTests
{
    public const string UserNonce = "myusernonce";
    public const string UserKey = "123456";
    public static readonly DateTime Timestamp = new(2022, 03, 21, 22, 52, 05);
    public const string ExpectedToken = "myusernonce:1647903125:458cfdb4e2e840423aff977cd1a0080a799531fa12fda6d3b05b8230e64b1b25";

    [Test]
    public void should_generate_valid_token()
    {
        DateTime CurrentTime() => Timestamp;
        var generator = new AuthTokenGenerator(UserNonce, UserKey, CurrentTime);

        var newToken = generator.GenerateToken();
        
        newToken.Should().Be(ExpectedToken);
    }

    [Test]
    public void should_generate_invalid_token_when_timestamp_is_different()
    {
        DateTime CurrentTime() => Timestamp.AddSeconds(1);
        var generator = new AuthTokenGenerator(UserNonce, UserKey, CurrentTime);

        var newToken = generator.GenerateToken();

        newToken.Should().NotBe(ExpectedToken);
    }
}