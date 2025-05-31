using System.Text.Json.Serialization;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek
{
    public class DeepSeekPayload
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonPropertyName("temperature")]
        public decimal? Temperature { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Parameters { get; set; } = new();

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}
