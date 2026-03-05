using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MyService.Endpoints;

namespace MyService.Tests;

/// <summary>
/// Tests for the status endpoint.
/// </summary>
public sealed class StatusEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StatusEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [Fact]
    public async Task GetStatus_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStatus_ReturnsJsonContentType()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/status");

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetStatus_ReturnsValidStatusResponse()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/status");
        var status = await response.Content.ReadFromJsonAsync<StatusResponse>();

        status.Should().NotBeNull();
        status!.ServiceName.Should().Be("MyService");
        status.Version.Should().Be("1.0.0");
        status.Status.Should().Be("Running");
        status.Environment.Should().NotBeNullOrWhiteSpace();
        status.Uptime.Should().NotBeNullOrWhiteSpace();
        status.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetStatus_UptimeFormat_LessThanMinute_ShowsSeconds()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/status");
        var status = await response.Content.ReadFromJsonAsync<StatusResponse>();

        status.Should().NotBeNull();
        status!.Uptime.Should().MatchRegex(@"\d+s");
    }

    [Fact]
    public async Task GetStatus_MultipleRequests_ReturnsConsistentServiceInfo()
    {
        var client = _factory.CreateClient();

        var response1 = await client.GetAsync("/status");
        var status1 = await response1.Content.ReadFromJsonAsync<StatusResponse>();

        await Task.Delay(100);

        var response2 = await client.GetAsync("/status");
        var status2 = await response2.Content.ReadFromJsonAsync<StatusResponse>();

        status1.Should().NotBeNull();
        status2.Should().NotBeNull();
        status1!.ServiceName.Should().Be(status2!.ServiceName);
        status1.Version.Should().Be(status2.Version);
        status1.Environment.Should().Be(status2.Environment);
    }
}
