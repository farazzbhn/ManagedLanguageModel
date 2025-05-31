using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Implementations.ResponseParsers;

internal class ExactStringLanguageModelResponseParser : ILanguageModelResponseParser<string>
{
    /// <summary>
    /// Attempts to parse the provided string by trimming any leading or trailing whitespace.
    /// </summary>
    /// <param name="input">The raw input string to be parsed.</param>
    /// <param name="parsed">The parsed (trimmed) result if successful, or null if parsing fails.</param>
    /// <returns>
    /// True if the input was successfully trimmed and parsed, otherwise false.
    /// </returns>
    public bool TryParse(string input, out string parsed)
    {
        try
        {
            parsed = input.Trim();  // Trim leading and trailing whitespaces from the input string
            return true;
        }
        catch (Exception)
        {
            parsed = null;
            return false;
        }
    }
}
