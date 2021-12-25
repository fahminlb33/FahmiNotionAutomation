using System;
using System.Linq;
using System.Threading.Tasks;
using FahmiNotionAutomation.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FahmiNotionAutomation
{
    public class AutoReporter
    {
        private readonly IKanbanService _kanban;
        private readonly INotionService _notion;
        private readonly IMongoStoreService _mongo;

        public AutoReporter(IKanbanService kanban, INotionService notion, IMongoStoreService mongo)
        {
            _kanban = kanban;
            _notion = notion;
            _mongo = mongo;
        }


        [FunctionName("CalculatePerformance")]
        public async Task CalculatePerformance([TimerTrigger("0 0 0 1 * *", RunOnStartup=true)] TimerInfo myTimer, ILogger log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            log.LogInformation($"Notion automation function started at: {DateTime.Now}");

            log.LogInformation("Loading Notion data...");
            var unarchivedCards = await _notion.GetUnarchivedTasks();
            var statistics = _kanban.GetStatistics(unarchivedCards);

            var latestPerformance = await _mongo.GetLatestStatistics();
            if (latestPerformance != null)
            {
                log.LogInformation("Calculating performance...");
                var performance = _kanban.GetPerformance(latestPerformance, statistics);

                log.LogInformation("Storing performance to MongoDB...");
                await _mongo.Store(performance);
            }

            log.LogInformation("Storing statistics to MongoDB...");
            await _mongo.Store(statistics);

            log.LogInformation("Set all cards to archived...");
            await Task.WhenAll(unarchivedCards.Select(async card => await _notion.SetToArchived(card)));

            log.LogInformation($"Notion automation function executed at: {DateTime.Now}");
        }
        
    }
}
