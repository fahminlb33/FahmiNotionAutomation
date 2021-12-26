using MongoDB.Bson;
using System;

namespace FahmiNotionAutomation.Infrastructure.Mongo
{
    public interface IMongoData
    {
        ObjectId Id { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
