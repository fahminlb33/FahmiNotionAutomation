using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FahmiNotionAutomation.Services
{
    public record KanbanPerformance
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public KanbanStatistics PreviousPeriod { get; set; }
        public KanbanStatistics CurrentPeriod { get; set; }
        public double CommitmentMovingAverage { get; set; }
        public double BurnedMovingAverage { get; set; }
        public double Change { get; set; }
    }
}
