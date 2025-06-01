using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Options;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek;
public class DeepSeekLlmClient : ILlmClient
{
    private readonly DeepSeekOptions _options;
    public DeepSeekLlmClient(IOptions<DeepSeekOptions> options)
    {
        _options = options.Value;
    }

    public async Task<LlmTransaction> InvokeAsync(string systemPrompt, IEnumerable<Message> history)
    {
        using var httpClient = new HttpClient();

        // add the system prompt to the very beginning of the messages
        var messages = new List<DeepSeekPayload.Message>()
        {
            new () { Role = "system", Content = systemPrompt }
        };

        foreach (Message message in history)
        {
            var msg = new DeepSeekPayload.Message()
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
        var payload = new DeepSeekPayload()
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
            Seed = _options.Seed
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
            throw new HttpRequestException($"DeepSeek API request failed with status {httpResponse.StatusCode}: {errorContent}");
        }

        // retrieve the content deserialize the content
        string response = await httpResponse.Content.ReadAsStringAsync();

        return new DeepSeekLLlmTransaction()
        {
            Request = jsonPayload,
            Response = response
        };
    }
}


