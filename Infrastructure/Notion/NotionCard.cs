using System;

namespace FahmiNotionAutomation.Infrastructure.Notion
{
    public class NotionCard
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public string Url { get; set; }
        public bool Archived { get; set; }

        public DateTime? StartDueAt { get; set; }
        public DateTime? EndDueAt { get; set; }
        public NotionCardPriority Priority { get; set; }
        public string[]? Category { get; set; }
        public int StoryPoints { get; set; }
        public NotionCardStatus Status { get; set; }
    }
}
