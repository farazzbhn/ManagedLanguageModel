using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedLib.LanguageModel.Implementations.Clients.Groq
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the <see cref="GroqLlmClient"/> as <br/>
        /// - scoped <see cref="ILlmClient"/> <br />
        /// - scoped <see cref="GroqLlmClient"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddGroq(this IServiceCollection services, GroqOptions options)
        {
            services.AddScoped<ILlmClient, GroqLlmClient>();
            services.AddScoped<GroqLlmClient>();
            services.AddSingleton(options);
            return services;
        }

    }
}
