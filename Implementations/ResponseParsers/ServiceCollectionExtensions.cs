using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedLib.LanguageModel.Implementations.ResponseParsers
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers built-in LanguageModelResponseParsers (e.g., ExactStringLanguageModelResponseParser) with the provided IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBuiltInLanguageModelParsers(this IServiceCollection services)
        {
            services.AddSingleton<ExactStringLanguageModelResponseParser>();
            return services;
        }

    }
}
