using FahmiNotionAutomation.Infrastructure.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FahmiNotionAutomation.Domain
{
    public record KanbanPerformance : IMongoData
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public KanbanStatistics PreviousPeriod { get; set; }
        public KanbanStatistics CurrentPeriod { get; set; }
        public double CommitmentMovingAverage { get; set; }
        public double BurnedMovingAverage { get; set; }
        public double Change { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
