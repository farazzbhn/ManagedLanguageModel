using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedLib.LanguageModel.Implementations.Clients.Groq
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the <see cref="GroqLanguageModelClient"/> as <br/>
        /// - scoped <see cref="ILanguageModelClient"/> <br />
        /// - scoped <see cref="GroqLanguageModelClient"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddGroq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILanguageModelClient, GroqLanguageModelClient>();
            services.AddScoped<GroqLanguageModelClient>();
            services.Configure<GroqOptions>(configuration.GetSection(nameof(GroqOptions)));
            return services;
        }

    }
}
