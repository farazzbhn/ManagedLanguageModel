using System.Text.Json;
using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek;
internal class DeepSeekLLlmTransaction : LlmTransaction
{
    public override string ExtractReply()
    {
        var responseObject = JsonSerializer.Deserialize<DeepSeekResponse>(Response);
        string answer = responseObject.Choices[0].Message.Content;
        return answer;
    }
}
