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

"DeepSeekOptions": 
{
    "model": "deepseek-chat",
    "ApiKey": "your-api-key"
    "ApiEndpoint": "https://api.deepseek.com/chat/completions",
    "parameters": { 
        "temperature": 0 
    }

},
"GroqOptions": 
{
    "model": "meta-llama/llama-4-scout-17b-16e-instruct",
    "ApiKey": "your-api-key"
    "ApiEndpoint": "https://api.groq.com/openai/v1/chat/completions",
    "parameters": { 
        "temperature": 0 
    }
},
"OpenAIOptions": {
    "model": "gpt-4o-mini-2024-07-18",
    "apiKey": "your-api-key"
    "apiEndpoint": "https://api.openai.com/v1/chat/completions",
    "parameters": { 
        "temperature": 0 
    }
}
```

### 2. ManagedLlmClient

The `ManagedLlmClient` provides built-in retry logic, error handling, and response validation & parsing capabilities:

** Note:**
* Inject `ILlmClient` directly if your dependency injection container has exactly one registered implementation.
* If multiple implementations of _ILlmClient_ exist (e.g., `OpenAIlLMClient`, `DeepseekLlmClient`, `GroqLlmClient`), inject the specific concrete type instead.

```csharp
public class WeatherService
{
    ILlmClient _llmClient;

    public WeatherService
    (
        OpenAILLmClient openAILlmClient, // Or inject ILlmClient if only one ILlmClient is registered
    )
    {
        _llmClient = openAILlmClient;
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

        WeatherForecast result = await new ManagedLlmClient(_llmClient).TryChatAsync(
            systemPrompt: prompt,
            history: history,
            parser: new WeatherForecastParser(),
            retries: 2
        );


        result.EnsureSuccess();

        return result.Parsed;
    }
}
```

### 3.  ILlmResponseHelper\<TResponse\> to the rescue !

One important aspect of the non-deterministic nature of Large Language Models is that they often generate unexpected tokens alongside the intended output. (e.g., "Here's your json data : ...")
While this is mostly acceptable, it becomes a challenge working in scenarios where structured data is required.

The proposed solution is to have the LLM clearly isolate the actual response in delimiters  (e.g, "Here's your json data ****  {"key":"value"} **** Do you need anything else ?")  and then try and parse
it into the desired C# type using regex, pattern matching and other techniques.
For the purpose, the library introduces the `ILlmResponseHelper<TResponse>` interface 

Sample request :

```
What’s the weather forecast for New York?  
Respond with a single word between ****  
Options: Sunny, Cloudy, Rainy, Snowy, Stormy, Foggy  

Example:  
****  
Sunny  
****  

```
Sample response : 

```
I predict the weather to be  ****  Sunny **** Do you need anything else ?
```
and response helpers to the rescue : 

```csharp
public class WeatherForecastParser : ILlmResponseHelper<WeatherForecast>
{
    public WeatherForecast Parse(string response)
    {
        var pattern = @"\*\*\*\*(.*?)\*\*\*\*";
        var match = Regex.Match(response, pattern, RegexOptions.Singleline);
        
        if (!match.Success)
        {
            throw new Exception(
                "Could not find weather forecast between **** delimiters",
                response
            );
        }

        var forecast = match.Groups[1].Value.Trim();
        return (WeatherForecast)Enum.Parse(typeof(WeatherForecast), forecast, ignoreCase: true);
    }
}

```
----------------------------------------------
Below is a list of built-in response parsers :
* `ExactStringLlmResponseHelper`

## Features

- Support for multiple LLM providers (OpenAI, DeepSeek, Groq)
- Standardized interface for all providers
- Built-in configuration management
- Response parsing capabilities
- Managed class for common operations with retry logic

## License

MIT