using System.Net;

using NSubstitute.Extensions;

using ProcrastiN8.Common;

namespace ProcrastiN8.Tests.Common;

/// <summary>
/// Unit tests for <see cref="OpenAIExcuseProvider"/>.
/// </summary>
public class OpenAIExcuseProviderTests
{
    [Fact]
    public async Task GetExcuse_Should_ReturnExcuse_When_ApiResponseIsValid()
    {
        // arrange
        var apiKey = "test-api-key";
        var httpClient = Substitute.For<HttpClient>();
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ \"choices\": [ { \"message\": { \"content\": \"I was debugging Schrödinger's cat.\" } } ] }")
        };
        httpClient.Configure().SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(responseMessage));

        var provider = new OpenAIExcuseProvider(apiKey, httpClient);

        // act
        var excuse = await provider.GetExcuseAsync();

        // assert
        Assert.Equal("I was debugging Schrödinger's cat.", excuse);
    }

    [Fact]
    public async Task GetExcuse_Should_ThrowException_When_ApiResponseIsInvalid()
    {
        // arrange
        var apiKey = "test-api-key";
        var httpClient = Substitute.For<HttpClient>();
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{ \"error\": \"Invalid request\" }")
        };
        httpClient.Configure().SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(responseMessage));

        var provider = new OpenAIExcuseProvider(apiKey, httpClient);

        // act & assert
        await Assert.ThrowsAsync<HttpRequestException>(() => provider.GetExcuseAsync());
    }
}