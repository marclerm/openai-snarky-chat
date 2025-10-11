namespace SnarkyChat.Api.Models
{
    public class OpenAIOptions
    {
        public string? BaseUrl { get; set; }
        public string? ApiKey { get; set; }  // optional; prefer env var
        public string Model { get; set; } = "gpt-5";
    }
}
