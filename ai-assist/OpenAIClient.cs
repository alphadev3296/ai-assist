using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ai_assist
{
    public class OpenAIClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string MediaType = "application/json";

        // Keeps the chat history
        private readonly List<Message> _messages = new();

        public OpenAIClient(string apiKey, string model)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        /// <summary>
        /// Add a message to the context (from user or system).
        /// Role must be "user", "assistant" or "system".
        /// </summary>
        public void AddMessage(string role, string content)
        {
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role must be specified", nameof(role));
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content must be specified", nameof(content));

            _messages.Add(new Message { Role = role, Content = content });
        }

        /// <summary>
        /// Clear the conversation context.
        /// </summary>
        public void ClearContext()
        {
            _messages.Clear();
        }

        /// <summary>
        /// Send the current context + optional new user message and get completion.
        /// </summary>
        /// <param name="userMessage">Optional new user message to add before sending</param>
        public async Task<string> ChatCompletionAsync(string? userMessage = null)
        {
            if (userMessage != null)
            {
                AddMessage("user", userMessage);
            }

            var requestBody = new
            {
                model = _model,
                messages = _messages
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, MediaType);

            var response = await _httpClient.PostAsync(ApiUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            var choice = doc.RootElement.GetProperty("choices")[0];
            var message = choice.GetProperty("message").GetProperty("content").GetString();

            // Add assistant response to context
            AddMessage("assistant", message ?? string.Empty);

            return message?.Trim() ?? string.Empty;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private class Message
        {
            public string Role { get; set; } = "";
            public string Content { get; set; } = "";
        }
    }
}
