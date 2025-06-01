using System.Text.Json.Serialization;

namespace ManagedLib.LanguageModel.Implementations.Clients.Groq
{
    public class GroqPayload
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonPropertyName("temperature")]
        public decimal? Temperature { get; set; }

        [JsonPropertyName("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }

        [JsonPropertyName("top_p")]
        public decimal? TopP { get; set; }

        [JsonPropertyName("stream")]
        public bool? Stream { get; set; }

        [JsonPropertyName("frequency_penalty")]
        public decimal? FrequencyPenalty { get; set; }

        [JsonPropertyName("presence_penalty")]
        public decimal? PresencePenalty { get; set; }

        [JsonPropertyName("stop")]
        public object? Stop { get; set; }

        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonPropertyName("seed")]
        public int? Seed { get; set; }

        [JsonPropertyName("n")]
        public int? N { get; set; }

        [JsonPropertyName("service_tier")]
        public string? ServiceTier { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Parameters { get; set; } = new();

        public class Message
        {
            [JsonPropertyName("role")]
            public string Role { get; set; }
            
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    }
}
