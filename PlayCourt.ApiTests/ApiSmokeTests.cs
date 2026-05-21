using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PlayCourt.ApiTests;

public sealed class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OpenApiJson_ReturnsSuccessAndDocumentShape()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var document = await response.Content.ReadFromJsonAsync<OpenApiDocumentResponse>();

        Assert.NotNull(document);
        Assert.False(string.IsNullOrWhiteSpace(document.OpenApi));
        Assert.NotNull(document.Info);
        Assert.NotNull(document.Paths);
    }

    [Fact]
    public async Task SwaggerUi_ReturnsHtml()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task WeatherForecast_ReturnsFiveItems()
    {
        var forecasts = await _client.GetFromJsonAsync<WeatherForecastResponse[]>("/weatherforecast");

        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);
        Assert.All(forecasts, forecast =>
        {
            Assert.NotEqual(default, forecast.Date);
            Assert.InRange(forecast.TemperatureC, -20, 54);
            Assert.NotNull(forecast.Summary);
        });
    }

    private sealed record OpenApiDocumentResponse(
        string OpenApi,
        object Info,
        Dictionary<string, object> Paths);

    private sealed record WeatherForecastResponse(
        DateOnly Date,
        int TemperatureC,
        int TemperatureF,
        string? Summary);
}
