using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.Exceptions;

namespace ManagedLib.LanguageModel.ManagedLlm;

public class ManagedLlmClient
{
    private readonly ILlmClient _client;

    public ManagedLlmClient(ILlmClient client)
    {
        _client = client;
    }

    public async Task<ManagedLlmResponse<TDestination>> TryChatAsync<TDestination>(
        string systemPrompt,
        IEnumerable<Message> history,
        ILlmResponseHelper<TDestination> parser,
        int retries = 2
    ) where TDestination : class
    {
        ArgumentOutOfRangeException.ThrowIfNegative(retries, nameof(retries));

        List<LlmTransaction> transactions = new();
        Exception innerException = new Exception("Inner exception not specified");

        for (int attempt = 1; attempt <= retries + 1; attempt++)
        {
            try
            {
                LlmTransaction result = await _client.InvokeAsync(systemPrompt, history);
                transactions.Add(result);

                string answer = result.ExtractReply();

                if (parser.TryParse(answer, out TDestination parsed))
                {
                    return new ManagedLlmResponse<TDestination>()
                    {
                        Parsed = parsed,
                        Transactions = transactions
                    };
                }
            }
            catch (Exception ex)
            {
                innerException = ex;

                if (attempt == retries)
                {
                    return new ManagedLlmResponse<TDestination>()
                    {
                        Parsed = null,
                        Exception = new MaxRetriesExceededException($"maximum number of retries exceeded for {_client.GetType().FullName}", innerException),
                        Transactions = transactions
                    };
                }
            }
        }

        return new ManagedLlmResponse<TDestination>()
        {
            Parsed = null,
            Exception = new MaxRetriesExceededException($"maximum number of retries exceeded for {_client.GetType().FullName}", innerException),
            Transactions = transactions
        };
    }
}