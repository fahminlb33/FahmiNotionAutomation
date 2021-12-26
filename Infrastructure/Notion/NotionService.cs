using FahmiNotionAutomation.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FahmiNotionAutomation.Infrastructure.Notion
{
    public interface INotionService
    {
        Task<IList<NotionCard>> GetUnarchivedTasks();
        Task SetToArchived(NotionCard card);
    }

    public class NotionService : INotionService
    {
        private readonly Config _config;
        private readonly HttpClient _httpClient;
        private readonly ILogger<INotionService> _logger;

        public NotionService(Config config, HttpClient httpClient, ILogger<INotionService> logger)
        {
            _config = config;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IList<NotionCard>> GetUnarchivedTasks()
        {
            const string QueryCardJsonBody = "{\"filter\":{\"property\":\"Archived\",\"checkbox\":{\"equals\":false}}}";
            var requestUri = $"https://api.notion.com/v1/databases/{_config.NotionWorklifeDatabaseId}/query";

            var cards = new List<NotionCard>();
            var jsonBody = await SendNotionRequest(HttpMethod.Post, requestUri, QueryCardJsonBody);
            foreach (var currentResult in jsonBody["results"])
            {
                var properties = currentResult["properties"];
                var card = new NotionCard();

                card.Id = currentResult["id"].Value<string>();
                card.CreatedAt = currentResult["created_time"].Value<DateTime>();
                card.LastEditedAt = currentResult["last_edited_time"].Value<DateTime>();
                card.Url = currentResult["url"].Value<string>();

                card.Title = properties["Name"]["title"][0]["plain_text"].Value<string>();
                card.Archived = properties["Archived"]["checkbox"].Value<bool>();

                var dueDate = properties["Due Date"]["date"];
                if (dueDate.HasValues)
                {
                    card.StartDueAt = dueDate["start"].Type != JTokenType.Null ? dueDate["start"].Value<DateTime>() : null;
                    card.EndDueAt = dueDate["end"].Type != JTokenType.Null ? dueDate["end"].Value<DateTime>() : null;
                }

                var category = properties["Category"]["multi_select"];
                if (category.HasValues)
                {
                    card.Category = category.Select(c => c["name"].Value<string>()).ToArray();
                }

                var priority = properties["Priority"]["select"];
                if (priority.HasValues)
                {
                    card.Priority = priority["name"].ToObject<NotionCardPriority>();
                }

                var storyPoints = properties["Story Points"]["number"];
                if (storyPoints.Type != JTokenType.Null)
                {
                    card.StoryPoints = storyPoints.Value<int>();
                }

                var status = properties["Status"]["select"];
                if (status.HasValues)
                {
                    card.Status = status["name"].ToObject<NotionCardStatus>();
                }

                cards.Add(card);
            }

            return cards;
        }

        public async Task SetToArchived(NotionCard card)
        {
            const string SetCardStatusJsonBody = "{\"properties\":{\"Archived\":{\"checkbox\":true}}}";
            var requestUri = $"https://api.notion.com/v1/pages/{card.Id}";

            await SendNotionRequest(HttpMethod.Post, requestUri, SetCardStatusJsonBody);
        }

        private async Task<JToken> SendNotionRequest(HttpMethod httpMethod, string requestUri, string body)
        {
            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Headers.Add("Authorization", _config.NotionAuthorizationToken);
            request.Headers.Add("Notion-Version", "2021-08-16");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var result = await _httpClient.SendAsync(request);

            using var responseBodyStream = await result.Content.ReadAsStreamAsync();
            using var textStream = new StreamReader(responseBodyStream);
            using var jsonReader = new JsonTextReader(textStream);

            var responseBody = JToken.Load(jsonReader);

            if (!result.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notion response is not success");
                _logger.LogDebug(responseBody.ToString());
            }

            result.EnsureSuccessStatusCode();

            return responseBody;
        }
    }
}
