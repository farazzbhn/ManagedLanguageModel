namespace ManagedLib.LanguageModel.Abstractions;
/// <summary>
/// Defines the contract for interacting with a language model (LLM).
/// </summary>
/// 
public interface ILlmClient
{
    /// <summary>
    /// Invokes the language model with a given prompt and message history, and returns the transaction result.
    /// </summary>
    /// <param name="systemPrompt">The input prompt to send to the language model for processing.</param>
    /// <param name="history">A collection of previous messages that provide context for the current prompt.</param>
    /// <param name="parameters">the list of parameters to override the injected configuration</param>
    /// <exception cref="Exception">  
    /// Throw an exception in case of a network or authorization error
    /// </exception>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is a <see cref="LlmTransaction"/> 
    /// containing the request sent, the response received, and any relevant extracted data.
    /// </returns>
    Task<LlmTransaction> InvokeAsync(string systemPrompt, IEnumerable<Message> history);
}

public class Message
{
    public Role From { get; set; }
    public string Content { get; set; }

    public Message(Role from, string content)
    {
        From = from;
        Content = content;
    }
}

public enum Role
{
    System,
    Assistant,
    User,
}