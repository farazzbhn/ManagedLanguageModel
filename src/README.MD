# ManagedLib.LanguageModel

A Managed .NET client for Large Language Models with built-in retry logic, error handling, and response validation 

## Features

- 🚀 Support for multiple LLM providers (OpenAI, DeepSeek, Groq)
- 🔄 Built-in retry logic and error handling
- ✨ Response validation and parsing capabilities
- 🛠️ Dependency injection ready
- 📝 Strongly typed responses
- 🔌 Extensible architecture for adding new providers


## Installation

```bash
dotnet add package ManagedLib.LanguageModel
```

## Getting Started

### 1. Configuration

Register your preferred LLM provider(s) in your `Program.cs` or `Startup.cs`. You may either instantiate configuration objects directly within the code or read & bind them from `appsettings.json`:

```csharp
// Add services to the container
var builder = WebApplication.CreateBuilder(args);

// Option 1: Direct Configuration in Code
builder.Services.AddGroq(
    new GroqOptions()
    {
        ApiEndpoint = "https://api.groq.com/openai/v1/chat/completions",
        Model = "meta-llama/llama-4-scout-17b-16e-instruct",
        ApiKey = "your-api-key",
    }
);

// Option 2: Read Configuration from appsettings.json
builder.Services.AddOpenAI(builder.Configuration.GetSection(nameof(OpenAIOptions)).Get<OpenAIOptions>()!);
builder.Services.AddDeepSeek(builder.Configuration.GetSection(nameof(DeepSeekOptions)).Get<DeepSeekOptions>()!);
```

Configure settings in `appsettings.json` (if used) :

The complete list of supported keys (parameters) can be found within each implemention.

```json
{
    "DeepSeekOptions": {
        "Model": "deepseek-chat",
        "ApiKey": "your-api-key",
        "ApiEndpoint": "https://api.deepseek.com/chat/completions",
        "Temperature": 0,
        "MaxTokens": 2000,
        "TopP": 1,
        "Stream": false,
    },
    "GroqOptions": {
        "Model": "meta-llama/llama-4-scout-17b-16e-instruct",
        "ApiKey": "your-api-key",
        "ApiEndpoint": "https://api.groq.com/openai/v1/chat/completions",
        "Temperature": 0,
        "MaxCompletionTokens": 2000,
        "TopP": 1,
    },
    "OpenAIOptions": {
        "Model": "gpt-4",
        "ApiKey": "your-api-key",
        "ApiEndpoint": "https://api.openai.com/v1/chat/completions",
        "Temperature": 0,
        "MaxTokens": 2000,
        "TopP": 1,
    }
}
```

### 2. ManagedLlmClient

The `ManagedLlmClient` provides built-in retry logic, error handling, and response validation & parsing capabilities:

**Note:**
* Inject `ILlmClient` directly if your dependency injection container has exactly one registered implementation.
* If multiple implementations of _ILlmClient_ exist (e.g., `OpenAILlmClient`, `DeepseekLlmClient`, `GroqLlmClient`), inject the specific concrete type instead.

Here's a complete example showing how to use the library with dependency injection and response parsing:

```csharp
using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.ManagedLlm;

// Get the LLM client from DI
using (var scope = app.Services.CreateScope())
{
    // Option 1: Get the default ILlmClient (when only one provider is registered)
    // ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<ILlmClient>();
    
    // Option 2: Get a specific provider's client
    ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<OpenAILlmClient>();
    // OR
    // ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<GroqLlmClient>();

    // Prepare your prompt
    var prompt =
        "classify the following product comment into exactly one of these below categories.\n" +
        "None, VeryUnsatisfied, Unsatisfied, Neutral, Satisfied, ExtremelySatisfied\n" +
        "Your response should be isolated between **** signs.\n" +
        "Example response: ****VeryUnsatisfied****";

    // Create message history
    List<Message> msgs = new()
    {
        new Message(Role.User, "I don't think I can really recommend the product to anyone I know! I mean, it is just super expensive")
    };

    // Use ManagedLlmClient for automatic retries and response parsing
    ManagedLlmResponse<FeedbackEnum> response = await new ManagedLlmClient(llmClient)
        .TryChatAsync(prompt, msgs, new FeedbackEnumParser());

    try
    {
        response.EnsureSuccess();
        Console.WriteLine($"User sentiment: {response.Parsed}");
    }
    catch
    {
        // Access transaction history for debugging
        foreach (var transaction in response.Transactions)
        {
            Console.WriteLine($"Request: {transaction.Request}");
            Console.WriteLine($"Response: {transaction.Response}\n");
        }
    }
}
```

### 3. ILlmResponseHelper\<TResponse\> to the rescue!

A key characteristic of LLMs (and GenAI systems in general) is their non-deterministic behaviour—they may produce varying outputs for the same input and can occasionally include unintended text alongside the expected result.

While this is generally acceptable for most cases, it can become problematic in scenarios that require strictly structured data, where any additional or unexpected tokens may break downstream processing.

The proposed solution is to 

1) Instruct the model to enclose the actual response within non-recurring delimiters so that worst case scenario response looks like :

    _"Here's your json output: **** {"prop1":"val1", "prop2":"val2"} **** ._


2) Extract and parse the response into strongly-typed C# types


For the purpose, the library introduces the `ILlmResponseHelper<TResponse>` interface.

Sample request:
```
Classify the following product comment into exactly one of these below categories.

None, VeryUnsatisfied, Unsatisfied, Neutral, Satisfied, ExtremelySatisfied

Your response should be isolated between **** signs

Example: ****VeryUnsatisfied****

Comment : I don't think I can really recommend the product to anyone I know! I mean, it is just super expensive
```

Sample response: 
```
Based on the user's comment expressing strong dissatisfaction with the product's price and explicitly stating they cannot recommend it to others, I classify this feedback as ****VeryUnsatisfied****
```

& Response helpers to the rescue: 

```csharp
public class FeedbackEnumParser : ILlmResponseHelper<FeedbackEnum>
{
    public bool TryParse(string response, out FeedbackEnum parsed)
    {
        parsed = default;
        var pattern = @"\*\*\*\*(.*?)\*\*\*\*";
        var match = Regex.Match(response, pattern, RegexOptions.Singleline);
        
        if (!match.Success)
        {
            return false;
        }

        var feedback = match.Groups[1].Value.Trim();
        return Enum.TryParse(feedback, true, out parsed);
    }
}
```

## Built-in Response Parsers
* `ExactStringLlmResponseHelper` - Extracts exact string matches between delimiters


## Raw Clients

```csharp
using (var scope = app.Services.CreateScope())
{
    LlmTransaction transaction = llmClient.InvokeAsync("Translate to French", new List<Message>(){new (Role.User, "I am a software engineer")}).Result;
    string response = transaction.ExtractReply();
    bool parsed = new ExactStringLlmResponseHelper().TryParse(response, out string translation);

    if (parsed)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" Translation : {translation.ToString()}");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Failed to parse the response.");
    }
}

```


## Project Structure

- `Abstractions/` - Core interfaces and base classes
- `Implementations/` - LLM provider implementations
- `ManagedLlm/` - Enhanced client with retry logic and response parsing
- `Exceptions/` - Custom exception types

## License

MIT