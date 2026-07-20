using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OrderFlow.Application.Dtos;
using OrderFlow.Simulator.Configuration;

namespace OrderFlow.Simulator.Services;

public sealed class OrderSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SimulatorSettings _settings;

    public OrderSender(
        IHttpClientFactory httpClientFactory,
        IOptions<SimulatorSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<bool> SendAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("OrderFlowApi");

        if (!string.IsNullOrWhiteSpace(_settings.BearerToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.BearerToken);
        }

        // Teste verifica token
        //Console.WriteLine($"API: {client.BaseAddress}");
        //Console.WriteLine($"Token vazio? {string.IsNullOrWhiteSpace(_settings.BearerToken)}");
        //Console.WriteLine($"Token início: {_settings.BearerToken[..Math.Min(20, _settings.BearerToken.Length)]}");
        //Console.WriteLine($"Authorization: {client.DefaultRequestHeaders.Authorization}");

        var response = await client.PostAsJsonAsync(
            "/api/orders",
            request,
            cancellationToken);

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine(
            $"ERRO | {request.ExternalReference} | Status: {(int)response.StatusCode} | {body}");

        return false;
    }
}