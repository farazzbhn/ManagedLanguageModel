using System.Text;
using System.Text.Json;
using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Options;

namespace ManagedLib.LanguageModel.Implementations.Clients.OpenAI;
public class OpenAILanguageModelClient : ILanguageModelClient
{
    private readonly OpenAIOptions _options;

    private OpenAILanguageModelClient(IOptions<OpenAIOptions> options)
    {
        _options = options.Value;
    }


    public async Task<LanguageModelTransaction> InvokeAsync(string systemPrompt, IEnumerable<Message> history)
    {
        using var httpClient = new HttpClient();


        // add the system prompt to the very beginning of the messages
        var messages = new List<OpenAIPayload.Message>()
        {
            new () { role = "system", content = systemPrompt }
        };


        foreach (Message message in history)
        {
            var msg = new OpenAIPayload.Message()
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
        var payload = new OpenAIPayload()
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


        return new OpenAILanguageModelTransaction()
        {
            Request = jsonPayload,
            Response = response
        };

    }


}


