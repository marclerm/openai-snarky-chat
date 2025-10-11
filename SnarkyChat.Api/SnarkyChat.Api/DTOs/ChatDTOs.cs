namespace SnarkyChat.Api.Models
{
    public class ChatRequest
    {
        public string UserMessage { get; set; } = "";
        //public string? SystemPrompt { get; set; }
        public string? Model { get; set; }
        public double? Temperature { get; set; }
    }

    public class ChatCompletionResponse
    {
        public List<Choice>? choices { get; set; }
        public class Choice
        {
            public Message? message { get; set; }
        }
        public class Message
        {
            public string? role { get; set; }
            public string? content { get; set; }
        }
    }
}
