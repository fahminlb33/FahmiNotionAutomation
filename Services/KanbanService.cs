using System;
using System.Collections.Generic;
using System.Linq;

namespace FahmiNotionAutomation.Services
{

    public interface IKanbanService
    {
        KanbanPerformance GetPerformance(KanbanStatistics previousPeriod, KanbanStatistics currentPeriod);
        KanbanStatistics GetStatistics(IEnumerable<NotionCard> cards);
    }

    public class KanbanService : IKanbanService
    {
        public KanbanStatistics GetStatistics(IEnumerable<NotionCard> cards)
        {
            return new KanbanStatistics
            {
                Period = DateTime.Now,
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

        public KanbanPerformance GetPerformance(KanbanStatistics previousPeriod, KanbanStatistics currentPeriod)
        {
            return new KanbanPerformance
            {
                PreviousPeriod = previousPeriod,
                CurrentPeriod = currentPeriod,
                CommitmentMovingAverage = (previousPeriod.Commitment + currentPeriod.Commitment) * 0.5,
                BurnedMovingAverage = (previousPeriod.Burned + currentPeriod.Burned) * 0.5,
            };
        }
    }
}
