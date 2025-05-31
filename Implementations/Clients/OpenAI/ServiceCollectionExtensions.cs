using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ManagedLib.LanguageModel.Implementations.Clients.OpenAI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the <see cref="OpenAILanguageModelClient"/> as <br/>
        /// - scoped <see cref="ILanguageModelClient"/> <br />
        /// - scoped <see cref="OpenAILanguageModelClient"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOpenAI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILanguageModelClient, OpenAILanguageModelClient>();
            services.AddScoped<OpenAILanguageModelClient>();
            services.Configure<OpenAIOptions>(configuration.GetSection(nameof(OpenAIOptions)));
            return services;
        }

    }
}
