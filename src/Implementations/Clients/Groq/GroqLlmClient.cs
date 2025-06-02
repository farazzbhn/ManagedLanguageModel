using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.Utilities;
using Microsoft.Extensions.Options;

namespace ManagedLib.LanguageModel.Implementations.Clients.Groq;
public class GroqLlmClient : ILlmClient
{
    private readonly GroqOptions _options;
    public GroqLlmClient(GroqOptions options)
    {
        _options = options;
    }

    public async Task<LlmTransaction> InvokeAsync(string systemPrompt, IEnumerable<Message> history)
    {
        using var httpClient = new HttpClient();

        // add the system prompt to the very beginning of the messages
        var messages = new List<GroqPayload.Message>()
        {
            new () { Role = "system", Content = systemPrompt }
        };

        foreach (Message message in history)
        {
            var msg = new GroqPayload.Message()
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
        var payload = new GroqPayload()
        {
            Model = _options.Model,
            Messages = messages,
            Temperature = _options.Temperature,
            MaxCompletionTokens = _options.MaxCompletionTokens,
            TopP = _options.TopP,
            Stream = _options.Stream,
            FrequencyPenalty = _options.FrequencyPenalty,
            PresencePenalty = _options.PresencePenalty,
            Stop = _options.Stop,
            User = _options.User,
            Seed = _options.Seed,
            N = _options.N,
            ServiceTier = _options.ServiceTier
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
            HttpErrorHandler.ThrowAppropriateException((int)httpResponse.StatusCode, errorContent, "Groq");
        }

        // retrieve the content deserialize the content
        string response = await httpResponse.Content.ReadAsStringAsync();

        return new GroqLlmTransaction()
        {
            Request = jsonPayload,
            Response = response
        };
    }
}


