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
    private const string DefaultPrompt = "Generate an excuse for procrastination.";
    private const string Endpoint = "https://api.openai.com/v1/chat/completions";
    private readonly string _apiKey = apiKey;
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public Task<string> GetExcuseAsync()
    {
        return GetExcuseAsync(DefaultPrompt, CancellationToken.None);
    }

    /// <summary>
    /// Generates an excuse with a custom prompt.
    /// </summary>
    public async Task<string> GetExcuseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are an excuse generator. Provide a creative excuse." },
                new { role = "user", content = string.IsNullOrWhiteSpace(prompt) ? DefaultPrompt : prompt }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No excuse available.";
    }
}
