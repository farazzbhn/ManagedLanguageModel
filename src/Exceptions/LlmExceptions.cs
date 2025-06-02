using System;

namespace ManagedLib.LanguageModel.Exceptions
{
    internal class AuthenticationException : Exception
    {
        internal AuthenticationException(string message) : base(message) { }
        internal AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    }

    internal class RateLimitException : Exception
    {
        internal RateLimitException(string message) : base(message) { }
        internal RateLimitException(string message, Exception innerException) : base(message, innerException) { }
    }

    internal class ServiceException : Exception
    {
        internal ServiceException(string message) : base(message) { }
        internal ServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
} 