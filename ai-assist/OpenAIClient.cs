using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json.Serialization;

namespace ai_assist
{
    public class OpenAIClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

        private readonly List<Message> _messages = new();

        /// <summary>
        /// Max approximate token count allowed in context (default 2000).
        /// </summary>
        private readonly int _maxTokens;

        public OpenAIClient(string apiKey, string model, int maxContextTokens = 2000)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _maxTokens = maxContextTokens;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "OpenAI-DotNet-Client/1.0");
        }

        /// <summary>
        /// Add a message to the context.
        /// </summary>
        public void AddMessage(string role, string content)
        {
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role must be specified", nameof(role));
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content must be specified", nameof(content));

            _messages.Add(new Message { Role = role, Content = content });
            TrimContextToMaxTokens();
        }

        /// <summary>
        /// Clear the conversation context.
        /// </summary>
        public void ClearContext()
        {
            _messages.Clear();
        }

        /// <summary>
        /// Estimate token count by word count as heuristic.
        /// </summary>
        private int EstimateTokens(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Estimate total tokens in the context and trim oldest messages until under limit.
        /// Preserves system messages always.
        /// </summary>
        private void TrimContextToMaxTokens()
        {
            int totalTokens = 0;
            // Count tokens backwards, keep system messages always
            for (int i = _messages.Count - 1; i >= 0; i--)
            {
                var msg = _messages[i];
                int tokens = EstimateTokens(msg.Content);
                totalTokens += tokens;

                if (totalTokens > _maxTokens && msg.Role != "system")
                {
                    // Remove this message
                    _messages.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Send the current context + optional new user message and get completion.
        /// </summary>
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
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            var choice = doc.RootElement.GetProperty("choices")[0];
            var message = choice.GetProperty("message").GetProperty("content").GetString();

            AddMessage("assistant", message ?? string.Empty);

            return message?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Streaming chat completion with partial chunks.
        /// Use await foreach on the result to receive chunks as they arrive.
        /// </summary>
        /// <param name="userMessage">New user message to add and send</param>
        /// <param name="cancellationToken"></param>
        public async IAsyncEnumerable<string> ChatCompletionStreamAsync(string userMessage, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            AddMessage("user", userMessage);

            var requestBody = new
            {
                model = _model,
                messages = _messages,
                stream = true
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl) { Content = content };
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new System.IO.StreamReader(stream);

            var assistantResponseBuilder = new StringBuilder();

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith("data: ")) continue;

                var jsonData = line.Substring("data: ".Length);
                if (jsonData.Trim() == "[DONE]") break;

                string? chunk = null;
                try
                {
                    chunk = ExtractContentChunk(jsonData);
                }
                catch (JsonException)
                {
                    // ignore malformed JSON chunk
                    chunk = null;
                }

                if (!string.IsNullOrEmpty(chunk))
                {
                    assistantResponseBuilder.Append(chunk);
                    yield return chunk;
                }
            }

            AddMessage("assistant", assistantResponseBuilder.ToString());
        }

        private string? ExtractContentChunk(string jsonData)
        {
            using var doc = JsonDocument.Parse(jsonData);
            var delta = doc.RootElement.GetProperty("choices")[0].GetProperty("delta");

            if (delta.TryGetProperty("content", out var contentElement))
            {
                return contentElement.GetString();
            }

            return null;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private class Message
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = "";

            [JsonPropertyName("content")]
            public string Content { get; set; } = "";
        }
    }
}
