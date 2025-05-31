using System.ComponentModel.DataAnnotations;

namespace ManagedLib.LanguageModel.Implementations.Clients.Groq;
public class GroqOptions
{
    [Required]
    public string Model { get; set; }

    [Required]
    public string ApiKey { get; set; }

    [Required]
    public string ApiEndpoint { get; set; }

    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}