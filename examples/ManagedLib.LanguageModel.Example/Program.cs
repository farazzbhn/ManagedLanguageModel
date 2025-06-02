using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.Example;
using ManagedLib.LanguageModel.Implementations.Clients.DeepSeek;
using ManagedLib.LanguageModel.Implementations.Clients.Groq;
using ManagedLib.LanguageModel.Implementations.Clients.OpenAI;
using ManagedLib.LanguageModel.Implementations.Helpers;
using ManagedLib.LanguageModel.ManagedLlm;
using Microsoft.Extensions.Configuration;
using Message = ManagedLib.LanguageModel.Abstractions.Message;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register different implementations of the ILLMClient.

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


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    string prompt =
        $"classify the following product comment into exactly one of these below categories . \n " +
        $"{string.Join(", ", Enum.GetNames(typeof(FeedbackEnum)))} \n" +
        $"Your response should be isolated between s****s signs \n" +
        $"Example response : ****s{Enum.GetNames(typeof(FeedbackEnum)).First()}s****";


    List<Message> msgs = new() 
    {
        new Message(Role.User,"I don't think I can really recommend the product to anyone I know ! I mean, it is super expensive ")
    };


    // Option 1: Get default ILlmClient (single provider) 
    // ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<ILlmClient>();

    // Option 2: Get specific provider's client
    ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<OpenAILlmClient>();
    //ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<OpenAILlmClient>();
    //ILlmClient llmClient = scope.ServiceProvider.GetRequiredService<GroqLlmClient>();



    #region Using ManagedLlmClient

    
    ManagedLlmResponse<FeedbackEnum> managedLlmResponse = new ManagedLlmClient(llmClient).TryChatAsync(prompt, msgs, new FeedbackEnumParser(), 3).Result;

    try
    {
        //throws an exception if not successful
        managedLlmResponse.EnsureSuccess();

        Console.WriteLine($"User sentiment : {managedLlmResponse.Parsed.ToString()}");
    }
    catch
    {
        // debugging purposes

        Console.WriteLine("ManagedLlmClient Encountered an error");

        for (int i = 0; i < managedLlmResponse.Transactions.Count; i++)
        {
            Console.WriteLine($" --[Transaction {i + 1}]------------------");
            Console.WriteLine($"req:\t{managedLlmResponse.Transactions[i].Request}");
            Console.WriteLine($"res:\t{managedLlmResponse.Transactions[i].Response}");
        }
    }
    #endregion


    // Using Raw Clients - single invocation ( no retries, no parsing helpers )

    LlmTransaction transaction = llmClient.InvokeAsync("Translate to French", new List<Message>() { new(Role.User, "I am a software engineer") }).Result;
    string response = transaction.ExtractReply();
    bool parsed = new ExactStringLlmResponseHelper().TryParse(response, out string translation);

    if (parsed)
    {
        Console.WriteLine($" Translation : {translation.ToString()}");
    }
    else
    {
        Console.WriteLine("Failed to translate the response.");
    }



}



