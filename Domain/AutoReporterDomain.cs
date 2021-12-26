using AutoMapper;
using FahmiNotionAutomation.Infrastructure.Mongo;
using FahmiNotionAutomation.Infrastructure.Notion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FahmiNotionAutomation.Domain
{
    public interface IAutoReporterDomain
    {
        Task ReportAsync();
        Task<IEnumerable<KanbanStatistics>?> GetAllStatistics();
        Task<IEnumerable<KanbanPerformance>?> GetAllPerformance();
    }

    public class AutoReporterDomain : IAutoReporterDomain
    {
        private readonly ILogger<IAutoReporterDomain> _log;
        private readonly IMapper _mapper;
        private readonly INotionService _notion;
        private readonly IMongoStoreService _mongo;

        private const string MongoCollectionStatistics = "statistics";
        private const string MongoCollectionPerformance = "performance";
        private const string MongoCollectionTasks = "tasks";

        public AutoReporterDomain(ILogger<IAutoReporterDomain> log, IMapper mapper, INotionService notion, IMongoStoreService mongo)
        {
            _log = log;
            _mapper = mapper;
            _notion = notion;
            _mongo = mongo;
        }

        public async Task ReportAsync()
        {
            _log.LogInformation("Loading Notion data...");
            var unarchivedCards = await _notion.GetUnarchivedTasks();

            var kanbanUnarchivedCards = _mapper.ProjectTo<KanbanCard>(unarchivedCards.AsQueryable()).ToList();
            var statistics = GetStatistics(kanbanUnarchivedCards);

            _log.LogInformation("Storing tasks...");
            await _mongo.StoreMany(kanbanUnarchivedCards, MongoCollectionTasks);

            var latestPerformance = await _mongo.GetLatest<KanbanStatistics>(MongoCollectionStatistics);
            if (latestPerformance != null)
            {
                _log.LogInformation("Calculating performance...");
                var performance = GetPerformance(latestPerformance, statistics);

                _log.LogInformation("Storing performance to MongoDB...");
                await _mongo.Store(performance, MongoCollectionPerformance);
            }

            _log.LogInformation("Storing statistics to MongoDB...");
            await _mongo.Store(statistics, MongoCollectionStatistics);

            _log.LogInformation("Set all cards to archived...");
            //await Task.WhenAll(unarchivedCards.Select(async card => await _notion.SetToArchived(card)));
        }

        public async Task<IEnumerable<KanbanStatistics>?> GetAllStatistics()
        {
            return await _mongo.GetAll<KanbanStatistics>(MongoCollectionStatistics);
        }

        public async Task<IEnumerable<KanbanPerformance>?> GetAllPerformance()
        {
            return await _mongo.GetAll<KanbanPerformance>(MongoCollectionPerformance);
        }

        private KanbanStatistics GetStatistics(IEnumerable<KanbanCard> cards)
        {
            return new KanbanStatistics
            {
                Period = DateTime.Now,
                CreatedAt = DateTime.Now,
                Backlog = cards.Where(x => x.Status == NotionCardStatus.Backlog).Sum(x => x.StoryPoints),
                Triage = cards.Where(x => x.Status == NotionCardStatus.Triage).Sum(x => x.StoryPoints),
                Todo = cards.Where(x => x.Status == NotionCardStatus.Todo).Sum(x => x.StoryPoints),
                InProgress = cards.Where(x => x.Status == NotionCardStatus.InProgress).Sum(x => x.StoryPoints),
                Review = cards.Where(x => x.Status == NotionCardStatus.Review).Sum(x => x.StoryPoints),
                Completed = cards.Where(x => x.Status == NotionCardStatus.Completed).Sum(x => x.StoryPoints),
                Commitment = cards
                    .Where(x => x.Status == NotionCardStatus.Todo ||
                        x.Status == NotionCardStatus.InProgress ||
                        x.Status == NotionCardStatus.Review ||
                        x.Status == NotionCardStatus.Completed)
                    .Sum(x => x.StoryPoints),
                Burned = cards
                    .Where(x => x.Status == NotionCardStatus.Completed)
                    .Sum(x => x.StoryPoints)
            };
        }

        private KanbanPerformance GetPerformance(KanbanStatistics previousPeriod, KanbanStatistics currentPeriod)
        {
            return new KanbanPerformance
            {
                CreatedAt = DateTime.Now,
                PreviousPeriod = previousPeriod,
                CurrentPeriod = currentPeriod,
                CommitmentMovingAverage = (previousPeriod.Commitment + currentPeriod.Commitment) * 0.5,
                BurnedMovingAverage = (previousPeriod.Burned + currentPeriod.Burned) * 0.5,
            };
        }
    }
}
