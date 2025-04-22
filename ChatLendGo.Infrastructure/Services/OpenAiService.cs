using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatLendGo.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatLendGo.Infrastructure.Services;

public class OpenAiService : ILlmProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiSettings _settings;

    public OpenAiService(HttpClient httpClient, IOptions<OpenAiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
    }

    public async Task<string> GenerateResponseAsync(string prompt, string systemInstruction, List<string> context = null)
    {
        var messages = new List<object>
        {
            new { role = "system", content = systemInstruction }
        };

        if (context != null && context.Count > 0)
        {
            foreach (var ctx in context)
            {
                messages.Add(new { role = "system", content = $"Additional context: {ctx}" });
            }
        }

        messages.Add(new { role = "user", content = prompt });

        return await SendRequestAsync(messages);
    }

    public async Task<string> GenerateResponseAsync(List<(string content, bool isFromUser)> messages, string systemInstruction)
    {
        var apiMessages = new List<object>
        {
            new { role = "system", content = systemInstruction }
        };

        foreach (var (content, isFromUser) in messages)
        {
            apiMessages.Add(new { role = isFromUser ? "user" : "assistant", content });
        }

        return await SendRequestAsync(apiMessages);
    }

    private async Task<string> SendRequestAsync(List<object> messages)
    {
        var requestBody = JsonSerializer.Serialize(new
        {
            model = _settings.Model,
            messages,
            temperature = 0.7,
            max_tokens = 1000
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API error: {response.StatusCode} - {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseString);

        return responseJson.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }
}

public class OpenAiSettings
{
    public string ApiKey { get; set; }
    public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    public string Model { get; set; } = "gpt-4-turbo";
}