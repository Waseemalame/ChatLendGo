using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatLendGo.Domain.Enums;
using ChatLendGo.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatLendGo.Infrastructure.Services;

public class OpenAiEmbeddingService : IEmbeddingProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiEmbeddingSettings _settings;

    public OpenAiEmbeddingService(HttpClient httpClient, IOptions<OpenAiEmbeddingSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
    }

    public EmbeddingType EmbeddingType => EmbeddingType.OpenAI;

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var requestBody = JsonSerializer.Serialize(new
        {
            model = _settings.Model,
            input = text
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI Embedding API error: {response.StatusCode} - {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseString);

        var embeddingData = responseJson.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding");

        var embedding = new float[embeddingData.GetArrayLength()];
        for (int i = 0; i < embedding.Length; i++)
        {
            embedding[i] = embeddingData[i].GetSingle();
        }

        return embedding;
    }
}

public class OpenAiEmbeddingSettings
{
    public string ApiKey { get; set; }
    public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/embeddings";
    public string Model { get; set; } = "text-embedding-3-small";
}