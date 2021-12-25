using dotenv.net;
using FahmiNotionAutomation.Infrastructure;
using FahmiNotionAutomation.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

[assembly: FunctionsStartup(typeof(FahmiNotionAutomation.AzureFunctions.Startup))]
namespace FahmiNotionAutomation.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            DotEnv.Load();
            var config = new Config();

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(config);
            builder.Services.AddTransient<INotionService, NotionService>();
            builder.Services.AddTransient<IKanbanService, KanbanService>();
            builder.Services.AddTransient<IMongoStoreService, MongoStoreService>();
            builder.Services.AddSingleton<IMongoClient>(new MongoClient(config.MongoUri));
        }
    }
}
