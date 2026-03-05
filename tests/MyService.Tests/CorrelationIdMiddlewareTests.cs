using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyService.Tests;

/// <summary>
/// Tests for the correlation ID middleware.
/// </summary>
public sealed class CorrelationIdMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly WebApplicationFactory<Program> _factory;

    public CorrelationIdMiddlewareTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Fact]
    public async Task Request_WithoutCorrelationId_GeneratesNewId()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/status");

        response.Headers.Should().ContainKey(CorrelationIdHeader);
        var correlationId = response.Headers.GetValues(CorrelationIdHeader).First();
        correlationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Request_WithCorrelationId_PreservesProvidedId()
    {
        var client = _factory.CreateClient();
        var expectedId = Guid.NewGuid().ToString("N");
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, expectedId);

        var response = await client.GetAsync("/status");

        response.Headers.Should().ContainKey(CorrelationIdHeader);
        var correlationId = response.Headers.GetValues(CorrelationIdHeader).First();
        correlationId.Should().Be(expectedId);
    }

    [Fact]
    public async Task MultipleRequests_WithoutCorrelationId_GeneratesDifferentIds()
    {
        var client = _factory.CreateClient();

        var response1 = await client.GetAsync("/status");
        var id1 = response1.Headers.GetValues(CorrelationIdHeader).First();

        var response2 = await client.GetAsync("/status");
        var id2 = response2.Headers.GetValues(CorrelationIdHeader).First();

        id1.Should().NotBe(id2);
    }

    [Fact]
    public async Task Request_WithEmptyCorrelationId_GeneratesNewId()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, string.Empty);

        var response = await client.GetAsync("/status");

        response.Headers.Should().ContainKey(CorrelationIdHeader);
        var correlationId = response.Headers.GetValues(CorrelationIdHeader).First();
        correlationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Request_WithWhitespaceCorrelationId_GeneratesNewId()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(CorrelationIdHeader, "   ");

        var response = await client.GetAsync("/status");

        response.Headers.Should().ContainKey(CorrelationIdHeader);
        var correlationId = response.Headers.GetValues(CorrelationIdHeader).First();
        correlationId.Should().NotBeNullOrWhiteSpace();
        correlationId.Trim().Should().Be(correlationId);
    }
}
