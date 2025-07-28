using System.Text;
using System.Text.Json;

namespace ProcrastiN8.Common;

/// <summary>
/// An implementation of <see cref="IExcuseProvider"/> that fetches excuses from OpenAI's ChatGPT API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OpenAIExcuseProvider"/> class.
/// </remarks>
/// <param name="apiKey">The API key for accessing OpenAI's ChatGPT API.</param>
/// <param name="httpClient">The HttpClient instance used for making API requests.</param>
public class OpenAIExcuseProvider(string apiKey, HttpClient httpClient) : IExcuseProvider
{
    private readonly string _apiKey = apiKey;
    private readonly HttpClient _httpClient = httpClient ?? new HttpClient();

    /// <inheritdoc />
    public async Task<string> GetExcuseAsync()
    {
        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are an excuse generator. Provide a creative excuse." },
                new { role = "user", content = "Generate an excuse for procrastination." }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No excuse available.";
    }
}