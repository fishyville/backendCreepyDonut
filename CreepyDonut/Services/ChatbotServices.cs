using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

public class OpenAiService
{
    private readonly string _apiKey;
    private readonly string _referer;
    private readonly string _model;
    private readonly HttpClient _httpClient;

    public OpenAiService(IConfiguration config)
    {
        _apiKey = config["OpenRouter:ApiKey"];
        _referer = config["OpenRouter:Referer"];   // Get from config
        _model = config["OpenRouter:Model"] ?? "mistralai/mistral-7b-instruct"; // Default fallback

        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", _referer);
        _httpClient.DefaultRequestHeaders.Add("X-Title", "Creepy Donut Assistant");
    }

    public async Task<string> AskChatGPT(string message, string donutList = "")
    {
        var prompt = $"""
        You are a friendly assistant for Creepy Donut. 
        Use the following donut menu to answer user questions:
        {donutList}

        User: {message}
        Assistant:
        """;

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant for a donut shop called Creepy Donut." },
                new { role = "user", content = prompt }
            },
            max_tokens = 150
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseContent);

        if (doc.RootElement.TryGetProperty("error", out var error))
        {
            var errorMessage = error.GetProperty("message").GetString();
            throw new Exception("OpenRouter API Error: " + errorMessage);
        }

        try
        {
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to parse OpenRouter response: " + ex.Message);
        }
    }
}
