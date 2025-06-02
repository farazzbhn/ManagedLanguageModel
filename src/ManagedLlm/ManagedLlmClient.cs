using ManagedLib.LanguageModel.Abstractions;
using ManagedLib.LanguageModel.Exceptions;
using System.Net.Http;

namespace ManagedLib.LanguageModel.ManagedLlm;

public class ManagedLlmClient
{
    private readonly ILlmClient _client;

    public ManagedLlmClient(ILlmClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Attempts to have a chat interaction with the language model with automatic retries and response parsing.
    /// </summary>
    /// <typeparam name="TDestination">The type to parse the LLM response into</typeparam>
    /// <param name="systemPrompt">The system prompt to guide the LLM's behavior</param>
    /// <param name="history">The conversation history</param>
    /// <param name="parser">Parser to convert the LLM response into the desired type</param>
    /// <param name="retries">Number of retries to attempt on retryable failures (default: 2)</param>
    /// <returns>A response object containing the parsed result, transaction history, and any terminal exceptions</returns>
    public async Task<ManagedLlmResponse<TDestination>> TryChatAsync<TDestination>(
        string systemPrompt,
        IEnumerable<Message> history,
        ILlmResponseHelper<TDestination> parser,
        int retries = 2
    )
    {
        ArgumentOutOfRangeException.ThrowIfNegative(retries, nameof(retries));

        List<LlmTransaction> transactions = new();
        var response = new ManagedLlmResponse<TDestination>() 
        { 
            Transactions = transactions,
            Parsed = default
        };

        for (int attempt = 1; attempt <= retries + 1; attempt++)
        {
            try
            {
                // might throw an exception (timeout , service error, authentication, etc.)
                LlmTransaction result = await _client.InvokeAsync(systemPrompt, history);

                transactions.Add(result);

                // cannot throw an exception
                string answer = result.ExtractReply();

                if (parser.TryParse(answer, out TDestination parsed))
                {
                    response.Parsed = parsed;
                    return response;
                }
                else
                {
                    // If parsing fails, we treat it as a failure and retry
                    throw new ParsingException($"Failed to parse {answer} to type {typeof(TDestination).Name}");
                }
            }
            catch (Exception ex)
            {
                // Only retry on timeouts, service errors, or parsing failures
                bool retryable =
                    ex is TimeoutException ||
                    ex is ServiceException ||
                    ex is ParsingException;

                // If it's not a retryable error or this is our last attempt, stop
                if (!retryable || attempt == retries + 1)
                {
                    if (attempt == retries + 1)
                    {
                        response.TerminalException = new MaxRetriesExceededException($"Maximum number of attempts exceeded for {_client.GetType().Name}\nInner Exception:\t{ex.Message}");
                    }
                    else // 
                    {
                        response.TerminalException = ex;
                    }

                    return response;
                }
            }
        }

        return response;
    }
}
