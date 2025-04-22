using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatLendGo.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatLendGo.Infrastructure.Services;

public class AnthropicService : ILlmProvider
{
    private readonly HttpClient _httpClient;
    private readonly AnthropicSettings _settings;

    public AnthropicService(HttpClient httpClient, IOptions<AnthropicSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    }

    public async Task<string> GenerateResponseAsync(string prompt, string systemInstruction, List<string> context = null)
    {
        var messageContent = prompt;

        if (context != null && context.Count > 0)
        {
            messageContent = $"Context information:\n{string.Join("\n", context)}\n\nUser query: {prompt}";
        }

        var requestBody = JsonSerializer.Serialize(new
        {
            model = _settings.Model,
            system = systemInstruction,
            messages = new[]
            {
                new { role = "user", content = messageContent }
            },
            max_tokens = 1000
        });

        return await SendRequestAsync(requestBody);
    }

    public async Task<string> GenerateResponseAsync(List<(string content, bool isFromUser)> messages, string systemInstruction)
    {
        var apiMessages = new List<object>();

        foreach (var (content, isFromUser) in messages)
        {
            apiMessages.Add(new { role = isFromUser ? "user" : "assistant", content });
        }

        var requestBody = JsonSerializer.Serialize(new
        {
            model = _settings.Model,
            system = systemInstruction,
            messages = apiMessages,
            max_tokens = 1000
        });

        return await SendRequestAsync(requestBody);
    }

    private async Task<string> SendRequestAsync(string requestBody)
    {
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Anthropic API error: {response.StatusCode} - {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseString);

        return responseJson.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();
    }
}

public class AnthropicSettings
{
    public string ApiKey { get; set; }
    public string ApiEndpoint { get; set; } = "https://api.anthropic.com/v1/messages";
    public string Model { get; set; } = "claude-3-opus-20240229";
}