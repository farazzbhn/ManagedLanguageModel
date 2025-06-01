namespace ManagedLib.LanguageModel.Exceptions
{
    public class MaxRetriesExceededException : Exception
    {
        public MaxRetriesExceededException()
            : base("The maximum number of retries exceeded")
        {
        }

        public MaxRetriesExceededException(string message)
            : base(message)
        {
        }

        public MaxRetriesExceededException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }

}
