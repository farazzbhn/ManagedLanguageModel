namespace ManagedLib.LanguageModel.Abstractions;


/// <summary>
/// Defines an interface for parsing a language model (LLM) response into a specified type.
/// Parsing an LLM response can involve more than just deserialization (e.g., handling special characters, 
/// using regular expressions, or other transformations depending on the format of the LLM output). 
/// This interface allows for such custom parsing logic to be implemented.
/// </summary>
/// <typeparam name="T">The type into which the LLM response will be parsed.</typeparam>
public interface ILanguageModelResponseParser<T>
{
    /// <summary>
    /// Attempts to parse the given input string into the specified type.
    /// </summary>
    /// <param name="input">The raw LLM response to be parsed.</param>
    /// <param name="parsed">The parsed output of the specified type, if the parsing is successful.</param>
    /// <returns>
    /// True if the input was successfully parsed, otherwise false.
    /// </returns>
    public bool TryParse(string input, out T parsed);
}