using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.Exceptions;

namespace ManagedLib.LanguageModel.Helpers;

public class LanguageModelHelper
{
    private readonly ILanguageModelClient _client;

    public LanguageModelHelper(ILanguageModelClient client)
    {
        _client = client;
    }

    public async Task<LanguageModelResponse<TDestination>> TryChatAsync<TDestination>(
        string systemPrompt,
        IEnumerable<Message> history,
        ILanguageModelResponseParser<TDestination> parser,
        int retries = 2
    ) where TDestination : class
    {
        ArgumentOutOfRangeException.ThrowIfNegative(retries, nameof(retries));

        List<LanguageModelTransaction> transactions = new();
        Exception innerException = new Exception("Inner exception not specified");

        for (int attempt = 1; attempt <= retries + 1; attempt++)
        {
            try
            {
                LanguageModelTransaction result = await _client.InvokeAsync(systemPrompt, history);
                transactions.Add(result);

                string answer = result.ExtractReply();

                if (parser.TryParse(answer, out TDestination parsed))
                {
                    return new LanguageModelResponse<TDestination>()
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
                    return new LanguageModelResponse<TDestination>()
                    {
                        Parsed = null,
                        Exception = new MaxRetriesExceededException($"maximum number of retries exceeded for {_client.GetType().FullName}", innerException),
                        Transactions = transactions
                    };
                }
            }
        }

        return new LanguageModelResponse<TDestination>()
        {
            Parsed = null,
            Exception = new MaxRetriesExceededException($"maximum number of retries exceeded for {_client.GetType().FullName}", innerException),
            Transactions = transactions
        };
    }
}