using System.Text.Json;
using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Implementations.Clients.Groq;
internal class GroqLlmTransaction : LlmTransaction
{
    public override string ExtractReply()
    {
        var responseObject = JsonSerializer.Deserialize<GroqResponse>(Response);

        // Extracting the llm response 
        string answer = responseObject.Choices[0].ChoiceMessage.Content;

        return answer;
    }
}
