using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace HomeBudget.Infrastructure.Email.Postmark
{
    public static class PostmarkConfiguration
    {
        public const string HTTP_CLIENT_NAME = "Postmark";

        public static IServiceCollection AddPostmarkHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PostmarkSettings>(configuration.GetSection("Postmark"));

            services.AddHttpClient(HTTP_CLIENT_NAME, (serviceProvider, c) =>
            {
                var postmarkSettings = serviceProvider.GetRequiredService<IOptions<PostmarkSettings>>().Value;

                c.BaseAddress = new Uri(postmarkSettings.ApiUrl!);
                c.DefaultRequestHeaders.Add("X-Postmark-Server-Token", postmarkSettings.ApiToken);
            });
            return services;
        }
    }
}