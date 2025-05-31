# ManagedLib.LanguageModel

A Managed .NET client for Large Language Models.

## Installation

```bash
dotnet add package ManagedLib.LanguageModel
```

## Quick Start

### 1. Configuration

Register your preferred LLM provider(s) in your `Program.cs` or `Startup.cs`:

```csharp
services.AddOpenAI(configuration); 
// AND/OR
services.AddDeepSeek(configuration);  
// AND/OR
services.AddGroq(configuration);  
```

Configure settings in `appsettings.json`:

```json
{
  "OpenAIOptions": {  
    "Model": "gpt-4o-mini-2024-07-18",
    "ApiKey": "your-api-key",
    "ApiEndpoint": "https://api.openai.com/v1/chat/completions",
    "Parameters": {
        "temperature": 0.1
    }
  }
}
```

### 2. LanguageModelHelper

The `LanguageModelHelper` provides built-in retry logic, error handling, and parsing capabilities:

```csharp
public class WeatherService
{
    private readonly LanguageModelHelper _helper;
    private readonly ILanguageModelResponseParser<WeatherForecast> _parser;

    public WeatherService(
        ILanguageModelClient languageModel,
        OpenAILanguageModelClient openAIClient, // in case there are multiple service registered
        ILanguageModelResponseParser<WeatherForecast> parser)
    {
        _helper = new LanguageModelHelper(languageModel);
        _parser = parser;
    }

    public async Task<WeatherForecast> GetWeatherForecast(string location)
    {
        var prompt = $"""
            What's the weather forecast for {location}?
            Your response should be a single weather condition between two **** signs.
            Choose from: Sunny, Cloudy, Rainy, Snowy, Stormy, Foggy
            
            For example:
            ****
            Sunny
            ****
            """;

        var history = new List<Message>();

        var result = await _helper.TryChatAsync(
            systemPrompt: prompt,
            history: history,
            parser: _parser,
            retries: 2
        );

        result.EnsureSuccess();
        return result.Parsed;
    }
}
```

### 3. Response Parsers

LLMs often add unexpected tokens or phrases like "Here's your response:" before the actual content. To handle this, we use delimiters in our prompts to reliably extract the exact content we need.

Example prompt with delimiters:
```
What's the weather forecast for New York?
Your response should be a single weather condition between two **** signs.
Choose from: Sunny, Cloudy, Rainy, Snowy, Stormy, Foggy

For example:
****
Sunny
****
```

Register built-in parsers:
```csharp
services.AddBuiltInLanguageModelResponseParsers();
```

Or register custom parsers:
```csharp
public class WeatherForecastParser : ILanguageModelResponseParser<WeatherForecast>
{
    public WeatherForecast Parse(string response)
    {
        var pattern = @"\*\*\*\*(.*?)\*\*\*\*";
        var match = Regex.Match(response, pattern, RegexOptions.Singleline);
        
        if (!match.Success)
        {
            throw new LanguageModelParseException(
                "Could not find weather forecast between **** delimiters",
                response
            );
        }

        var forecast = match.Groups[1].Value.Trim();
        return (WeatherForecast)Enum.Parse(typeof(WeatherForecast), forecast, ignoreCase: true);
    }
}

// Register the parser
services.AddSingleton<ILanguageModelResponseParser<WeatherForecast>, WeatherForecastParser>();
```

## Unwrapped Clients

Here's an example showing how to use both the generic `ILanguageModelClient` and a specific provider client:

You may use the ILanguageModelClient injection if no more than one language model is registered within the service collection.

```csharp
public class TranslationService
{
    private readonly ILanguageModelClient _genericClient;

    private readonly DeepSeekLanguageModelClient _deepSeekClient;

    public TranslationService(
        ILanguageModelClient genericClient,
        DeepSeekLanguageModelClient deepSeekClient)
    {
        _genericClient = genericClient;
        _deepSeekClient = deepSeekClient;
    }

    // Using the generic client for basic translations
    public async Task<string> TranslateBasic(string text, string targetLanguage)
    {
        var prompt = $"""
            Translate the following text to {targetLanguage}:
            {text}
            Your response should be the translation between **** signs.
            
            For example:
            ****
            Translated text here
            ****
            """;

        var response = await _genericClient.InvokeAsync(prompt, new List<Message>());
        return response.Response;
    }

    // Using DeepSeek specifically for technical translations
    public async Task<string> TranslateTechnical(string text, string targetLanguage)
    {
        var prompt = $"""
            Translate the following technical text to {targetLanguage}, maintaining all technical terms:
            {text}
            Your response should be the translation between **** signs.
            
            For example:
            ****
            Translated technical text here
            ****
            """;

        var response = await _deepSeekClient.InvokeAsync(prompt, new List<Message>());
        return response.Response;
    }
}
```

## Features

- Support for multiple LLM providers (OpenAI, DeepSeek, Groq)
- Standardized interface for all providers
- Built-in configuration management
- Response parsing capabilities
- Helper class for common operations with retry logic
- Flexible registration allowing both generic and specific provider usage

## License

MIT