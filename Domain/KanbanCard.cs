using AutoMapper;
using FahmiNotionAutomation.Infrastructure.Mongo;
using FahmiNotionAutomation.Infrastructure.Notion;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FahmiNotionAutomation.Domain
{
    public record KanbanCard : IMongoData
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string NotionPageId { get; set; }
        public string Title { get; set; }
        public DateTime TaskCreatedAt { get; set; }
        public DateTime TaskLastEditedAt { get; set; }
        public string Url { get; set; }
        public bool Archived { get; set; }

        public DateTime? StartDueAt { get; set; }
        public DateTime? EndDueAt { get; set; }
        public NotionCardPriority Priority { get; set; }
        public string[]? Category { get; set; }
        public int StoryPoints { get; set; }
        public NotionCardStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class KanbanCardMapperProfile : Profile
    {
        public KanbanCardMapperProfile()
        {
            CreateMap<NotionCard, KanbanCard>()
                .ForMember(dest => dest.Id, options => options.Ignore())
                .ForMember(dest => dest.NotionPageId, options => options.MapFrom(src => src.Id));
        }
    }
}
