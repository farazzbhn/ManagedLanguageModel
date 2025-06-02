using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.Exceptions;
using ManagedLib.LanguageModel.Utilities;

namespace ManagedLib.LanguageModel.Implementations.Clients.OpenAI;
public class OpenAILlmClient : ILlmClient
{
    private readonly OpenAIOptions _options;

    public OpenAILlmClient(OpenAIOptions options)
    {
        _options = options;
    }

    public async Task<LlmTransaction> InvokeAsync(string systemPrompt, IEnumerable<Message> history)
    {
        using var httpClient = new HttpClient();

        // add the system prompt to the very beginning of the messages
        var messages = new List<OpenAIPayload.Message>()
        {
            new () { Role = "system", Content = systemPrompt }
        };

        foreach (Message message in history)
        {
            var msg = new OpenAIPayload.Message()
            {
                Role = message.From switch
                {
                    Role.User => "user",
                    Role.System => "system",
                    Role.Assistant => "assistant",
                    _ => throw new ArgumentException($"Invalid Role: {message.From}")
                },
                Content = message.Content
            };
            messages.Add(msg);
        }

        // invoke the endpoint
        var payload = new OpenAIPayload()
        {
            Model = _options.Model,
            Messages = messages,
            Temperature = _options.Temperature,
            MaxTokens = _options.MaxTokens,
            TopP = _options.TopP,
            Stream = _options.Stream,
            FrequencyPenalty = _options.FrequencyPenalty,
            PresencePenalty = _options.PresencePenalty,
            Stop = _options.Stop,
            User = _options.User,
            Seed = _options.Seed,
            N = _options.N,
            ResponseFormat = _options.ResponseFormat,
            Tools = _options.Tools,
            ToolChoice = _options.ToolChoice
        };

        string jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions 
        { 
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
        });
        StringContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Create the HTTP request
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.ApiEndpoint)
        {
            Headers =
            {
                { "Authorization", $"Bearer {_options.ApiKey}" }
            },
            Content = httpContent
        };

        // Send the request and get the response
        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            HttpErrorHandler.ThrowAppropriateException((int)httpResponse.StatusCode, errorContent, "OpenAI");
        }

        // retrieve the content deserialize the content
        string response = await httpResponse.Content.ReadAsStringAsync();

        return new OpenAILlmTransaction()
        {
            Request = jsonPayload,
            Response = response
        };
    }
}


