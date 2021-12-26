using dotenv.net;
using FahmiNotionAutomation.Domain;
using FahmiNotionAutomation.Infrastructure;
using FahmiNotionAutomation.Infrastructure.Mongo;
using FahmiNotionAutomation.Infrastructure.Notion;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;

[assembly: FunctionsStartup(typeof(FahmiNotionAutomation.AzureFunctions.Startup))]
namespace FahmiNotionAutomation.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            DotEnv.Load();
            var config = new Config();

            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddSingleton(config);
            builder.Services.AddScoped<INotionService, NotionService>();
            builder.Services.AddScoped<IMongoStoreService, MongoStoreService>();
            builder.Services.AddScoped<IAutoReporterDomain, AutoReporterDomain>();
            builder.Services.AddSingleton<IMongoClient>(new MongoClient(config.MongoUri));
        }
    }
}
