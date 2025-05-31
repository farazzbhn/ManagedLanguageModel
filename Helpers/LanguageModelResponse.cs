using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.Helpers
{
    public sealed class LanguageModelResponse<T> where T : class
    {
        public bool IsParsed => Parsed is not null && Parsed != default;
        public T? Parsed { get; set; }
        public IEnumerable<LanguageModelTransaction> Transactions { get; set; }
        public Exception? Exception { get; set; }


        /// <summary>
        /// throws an exception in case there is no parsed response of type T available within the object
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void EnsureSuccess()
        {
            if (!IsParsed) throw new Exception("Failed");
        }
    }
}
