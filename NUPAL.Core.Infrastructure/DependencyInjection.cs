using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUPAL.Core.Application.Interfaces;
using Nupal.Core.Infrastructure.Repositories;

namespace NUPAL.Core.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoUrl = configuration.GetValue<string>("MONGO_URL")
                           ?? Environment.GetEnvironmentVariable("MONGO_URL");

            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoUrl));
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase("nupal");
            });

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();

            // Register Wuzzuf job scraping service
            services.AddHttpClient<IJobService, Services.WuzzufJobService>();

            return services;
        }
    }
}
