using System.Net.Http.Json;
using System.Text.Json;

namespace GSService.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = "AIzaSyABEYV3wyt485pndWFF163blncdfk85Xmc";  // 🔥 put your real key here
        }

        public async Task<string?> GetGeminiResponse(string userPrompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = userPrompt }
                        }
                    }
                }
            };

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode}";
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var doc = JsonDocument.Parse(json);
                var text = doc.RootElement
                               .GetProperty("candidates")[0]
                               .GetProperty("content")
                               .GetProperty("parts")[0]
                               .GetProperty("text")
                               .GetString();

                return text;
            }
            catch
            {
                return "Error parsing response.";
            }
        }
    }
}
