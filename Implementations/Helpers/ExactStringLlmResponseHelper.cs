using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Implementations.Helpers;

public class ExactStringLlmResponseHelper : ILlmResponseHelper<string>
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
        if (input is null)
        {
            parsed = null;
            return false;
        }

        parsed = input.Trim();
        return true;
    }
}
