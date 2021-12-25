using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FahmiNotionAutomation.Services
{
    public record KanbanStatistics
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime Period { get; set; }
        public int Backlog { get; set; }
        public int Triage { get; set; }
        public int Todo { get; set; }
        public int InProgress { get; set; }
        public int Review { get; set; }
        public int Completed { get; set; }
        public int Commitment { get; set; }
        public int Burned { get; set; }
        public int RemainingCommitment => Commitment - Burned;
    }
}
