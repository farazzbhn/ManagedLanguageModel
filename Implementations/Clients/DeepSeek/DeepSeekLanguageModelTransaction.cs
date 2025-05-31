using System.Text.Json;
using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek;
internal class DeepSeekLanguageModelTransaction : LanguageModelTransaction
{
    public override string ExtractReply()
    {
        dynamic? responseObject = JsonSerializer.Deserialize<dynamic>(Response);

        // Extracting the llm response 
        string answer = responseObject?.choices?[0]?.message?.content?.ToString()!;

        return answer;
    }
}
