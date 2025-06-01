
using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the <see cref="DeepSeekLlmClient"/> as <br/>
        /// - scoped <see cref="ILlmClient"/> <br />
        /// - scoped <see cref="DeepSeekLlmClient"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDeepSeek(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILlmClient, DeepSeekLlmClient>();
            services.AddScoped<DeepSeekLlmClient>();
            services.Configure<DeepSeekOptions>(configuration.GetSection(nameof(DeepSeekOptions)));
            return services;
        }

    }
}
