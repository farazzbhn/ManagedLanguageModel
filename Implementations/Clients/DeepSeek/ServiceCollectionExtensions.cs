
using ManagedLib.LanguageModel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedLib.LanguageModel.Implementations.Clients.DeepSeek
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the <see cref="DeepSeekLanguageModelClient"/> as <br/>
        /// - scoped <see cref="ILanguageModelClient"/> <br />
        /// - scoped <see cref="DeepSeekLanguageModelClient"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDeepSeek(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILanguageModelClient, DeepSeekLanguageModelClient>();
            services.AddScoped<DeepSeekLanguageModelClient>();
            services.Configure<DeepSeekOptions>(configuration.GetSection(nameof(DeepSeekOptions)));
            return services;
        }

    }
}
