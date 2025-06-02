using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ManagedLib.LanguageModel.Implementations.Clients.OpenAI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the <see cref="OpenAILlmClient"/> as <br/>
        /// - scoped <see cref="ILlmClient"/> <br />
        /// - scoped <see cref="OpenAILlmClient"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddOpenAI(this IServiceCollection services, OpenAIOptions options)
        {
            services.AddScoped<ILlmClient, OpenAILlmClient>();
            services.AddScoped<OpenAILlmClient>();
            services.AddSingleton<OpenAIOptions>(options);
            return services;
        }

    }
}
