using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            new DeepSeekPayload.Message() { role = "system", content = systemPrompt }
        };


        foreach (Message message in history)
        {
            var msg = new DeepSeekPayload.Message()
            {
                role = message.From switch
                {
                    Role.User => "user",
                    Role.System => "system",
                    Role.Assistant => "assistant",
                    _ => throw new ArgumentException($"Invalid Role: {message.From}")
                },
                content = message.Content
            };
            messages.Add(msg);
        }


        // invoke the endpoint
        var payload = new DeepSeekPayload()
        {
            Model = _options.Model,
            Messages = messages,
            Parameters = _options.Parameters?.ToDictionary(
                kvp => kvp.Key,
                kvp => (object)kvp.Value
            ) ?? new()
        };

        string jsonPayload = JsonSerializer.Serialize(payload);
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
        httpResponse.EnsureSuccessStatusCode();

        // retrieve the content deserialize the content
        string response = await httpResponse.Content.ReadAsStringAsync();


        return new DeepSeekLLlmTransaction()
        {
            Request = jsonPayload,
            Response = response
        };

    }


}


