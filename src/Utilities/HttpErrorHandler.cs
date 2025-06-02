using System.Net.Http;
using ManagedLib.LanguageModel.Exceptions;

namespace ManagedLib.LanguageModel.Utilities;

public static class HttpErrorHandler
{
    public static void ThrowAppropriateException(int statusCode, string errorContent, string provider)
    {
        switch (statusCode)
        {
            case 401: // Unauthorized
            case 403: // Forbidden
                throw new AuthenticationException($"{provider} authentication failed with status {statusCode}: {errorContent}");
            
            case 408: // Request Timeout
            case 504: // Gateway Timeout
                throw new TimeoutException($"{provider} request timed out with status {statusCode}: {errorContent}");

            case 429: // Too Many Requests
                throw new RateLimitException($"{provider} rate limit exceeded: {errorContent}");

            case >= 500: // Server errors (except 504 which is handled above)
                throw new ServiceException($"{provider} service error {statusCode}: {errorContent}");

            default:
                throw new HttpRequestException($"{provider} API request failed with status {statusCode}: {errorContent}");
        }
    }
} 