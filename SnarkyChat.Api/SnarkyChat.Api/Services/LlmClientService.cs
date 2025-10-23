using Microsoft.Extensions.Options;
using SnarkyChat.Api.Models;
using static System.Net.WebRequestMethods;

namespace SnarkyChat.Api.Services
{
    public class LlmClientService
    {
        private readonly HttpClient _http;
        private readonly OpenAIOptions _opts;
        private readonly string _version = "v1";

        public LlmClientService(IHttpClientFactory factory, IOptions<OpenAIOptions> opts)
        {
            _http = factory.CreateClient("openai");
            _opts = opts.Value;
        }

        /// <summary>
        /// Sends a chat request to the LLM and returns the response as a string.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        internal async Task<string> ChatAsync(ChatRequest request, CancellationToken ct = default)
        {
            var model = string.IsNullOrWhiteSpace(request.Model) ? _opts.Model : request.Model;
            var messages = new List<object>
            {
                new { role = "system", content = GetSystemPrompt() }
            };

            if(request.Messages != null)
            {
                foreach(var msg in request.Messages)
                {
                    messages.Add(new { role = msg.role, content = msg.content });
                }
            }

            var body = new
            {
                model,
                messages,
                temperature = request.Temperature ?? 0.2
            };

            using var resp = await _http.PostAsJsonAsync($"{_version}/chat/completions", body, ct);
            resp.EnsureSuccessStatusCode();

            var payload = await resp.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: ct);
            return payload?.choices?.FirstOrDefault()?.message?.content ?? "";
        }

        /// <summary>
        /// Streams response tokens directly to the HttpResponse as a text/event-stream.
        /// As usually ChatGPT does
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="res">Response</param>
        /// <returns></returns>
        internal async Task StreamAsync(ChatRequest req, HttpResponse res, CancellationToken ct = default)
        {
            var model = string.IsNullOrWhiteSpace(req.Model) ? _opts.Model : req.Model;

            var body = new
            {
                model,
                stream = true,
                messages = new object[]
                {
                    new { role = "system", content = GetSystemPrompt() },
                    new { role = "user", content = req.UserMessage }
                }
            };

            res.Headers.ContentType = "text/event-stream";
            using var apiResp = await _http.PostAsJsonAsync($"{_version}/chat/completions", body, ct);
            apiResp.EnsureSuccessStatusCode();

            using var stream = await apiResp.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                // OpenAI streams lines prefixed with "data: {json}" and ends with [DONE]
                if (line.StartsWith("data: "))
                {
                    var json = line["data: ".Length..].Trim();
                    if (json == "[DONE]") break;

                    // Parse minimal delta (avoid heavy models)
                    var chunk = System.Text.Json.JsonDocument.Parse(json);
                    var delta = chunk.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("delta");

                    if (delta.TryGetProperty("content", out var contentElem))
                    {
                        var token = contentElem.GetString();
                        if (!string.IsNullOrEmpty(token))
                        {
                            await res.WriteAsync($"data: {token}\n\n");
                            await res.Body.FlushAsync();
                        }
                    }
                }
            }
        }

        // Get the system prompt, can be customized or extended to accept from request
        private string GetSystemPrompt()
        {
            // You can customize this prompt to change the AI's behavior
            // For example, make it more or less snarky, or change its personality
            return "You're a delightfully snarky, razor-sharp, and hilariously sarcastic AI assistant with a flair for irony and a talent for roasting questions just enough to keep things entertaining but never mean.";
        }

    }
}
