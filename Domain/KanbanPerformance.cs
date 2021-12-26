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
        public DateTime PreviousPeriod { get; set; }
        public DateTime CurrentPeriod { get; set; }
        public double Commitment { get; set; }
        public double CommitmentChange { get; set; }
        public double CommitmentMovingAverage { get; set; }
        public double Burned { get; set; }
        public double BurnedChange { get; set; }
        public double BurnedMovingAverage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
