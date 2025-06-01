using System.Text.Json;
using ManagedLib.LanguageModel.Abstractions;
namespace ManagedLib.LanguageModel.Implementations.Clients.OpenAI;

internal class OpenAILlmTransaction : LlmTransaction
{
    public override string ExtractReply()
    {
        OpenAIResponse responseObject = JsonSerializer.Deserialize<OpenAIResponse>(Response)!;

        // Extracting the llm response 
        string answer = responseObject!.Choices[0].Message.Content;

        return answer;
    }
}
