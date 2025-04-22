using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ChatLendGo.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatLendGo.Infrastructure.Services;

public class WhisperService
{
    private readonly HttpClient _httpClient;
    private readonly WhisperSettings _settings;

    public WhisperService(HttpClient httpClient, IOptions<WhisperSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
    }

    public async Task<string> TranscribeAudioAsync(byte[] audioData)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(audioData);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

        content.Add(fileContent, "file", "audio.wav");
        content.Add(new StringContent(_settings.Model), "model");

        var response = await _httpClient.PostAsync(_settings.ApiEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Whisper API error: {response.StatusCode} - {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseString);

        return responseJson.RootElement.GetProperty("text").GetString();
    }
}

public class WhisperSettings
{
    public string ApiKey { get; set; }
    public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/audio/transcriptions";
    public string Model { get; set; } = "whisper-1";
}