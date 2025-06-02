using System.Text.RegularExpressions;
using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Example;

public class FeedbackEnumParser : ILlmResponseHelper<FeedbackEnum>
{
    public bool TryParse(string input, out FeedbackEnum parsed)
    {
        parsed = default;

        // Extract time string between *** and *** using regex
        var match = Regex.Match(input, @"\*{4}(.*?)\*{4}");

        if (match.Success)
        {
            string sentiment = match.Groups[1].Value;

            bool valid = Enum.TryParse<FeedbackEnum>(sentiment, ignoreCase: true, out FeedbackEnum result);

            parsed = valid ? result : parsed;

            return valid;
        }

        return false;
    }

}
