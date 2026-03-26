using System.Net.Http.Json;
using System.Collections.Generic;

namespace FinancialTwin.API.Services{
    public interface IAiService
    {
        Task<string> GetAdviceAsync(string prompt);
    }

    public class MockAiService : IAiService
    {
        public Task<string> GetAdviceAsync(string prompt)
        {
            return Task.FromResult($"[MockAI] Received: {prompt}. Your financial future looks bright! Save consistently and let compound interest work for you.");
        }
    }


    public class GeminiAiService : IAiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        
        public GeminiAiService(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = configuration["GeminiAi:ApiKey"] ?? "";
            _httpClient = httpClient;
        }

        public async Task<string> GetAdviceAsync(string prompt)
        {
            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={_apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonData>();
            return result?.candidates?[0]?.content?.parts?[0]?.text ?? "No response from AI";
        }

        // Helper classes to deserialize Gemini response
        private class JsonData 
        {
            public List<Candidate>? candidates { get; set; }
        }
        private class Candidate { public Content? content { get; set; } }
        private class Content { public List<Part>? parts { get; set; } }
        private class Part { public string? text { get; set; } }
    }      
}