using System.ComponentModel.DataAnnotations;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek;
public class DeepSeekOptions
{
    [Required]
    public string Model { get; set; }

    [Required]
    public string ApiKey { get; set; }

    [Required]
    public string ApiEndpoint { get; set; }

    public decimal? Temperature { get; set; } // Optional, defaults to 1

    public int? MaxTokens { get; set; } // Optional

    public decimal? TopP { get; set; } // Optional, defaults to 1

    public bool? Stream { get; set; } // Optional, defaults to false

    public decimal? FrequencyPenalty { get; set; } // Optional, defaults to 0

    public decimal? PresencePenalty { get; set; } // Optional, defaults to 0

    public object? Stop { get; set; } // Optional, can be string or string[]

    public string? User { get; set; } // Optional

    public int? Seed { get; set; } // Optional
}
