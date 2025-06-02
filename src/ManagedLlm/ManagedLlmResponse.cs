using ManagedLib.LanguageModel.Abstractions;

namespace ManagedLib.LanguageModel.ManagedLlm
{
    public sealed class ManagedLlmResponse<T>
    {
        public bool IsParsed => !EqualityComparer<T>.Default.Equals(Parsed, default);
        public T Parsed { get; set; }
        public List<LlmTransaction> Transactions { get; set; }
        public Exception? TerminalException { get; set; }

        /// <summary>
        /// throws an exception in case there is no parsed response of type T available within the object
        /// </summary>
        /// <exception cref="TerminalException"></exception>
        public void EnsureSuccess() 
        {
            if (!IsParsed) throw new Exception("Failed");
        }
    }
}
