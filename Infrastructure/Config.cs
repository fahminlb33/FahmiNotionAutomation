using System;

namespace FahmiNotionAutomation.Infrastructure
{
    public class Config
    {
        public string ApplicationName { get; }
        public string NotionAuthorizationToken { get; }
        public string NotionWorklifeDatabaseId { get; }
        public string WorklifeGSheetId { get; }
        public string MongoUri { get; }
        public string MongoDatabase { get; }

        public Config()
        {
            ApplicationName = "Fahmi's Automation";
            NotionAuthorizationToken = GetEnvironmentVariable("NOTION_AUTHORIZATION_TOKEN");
            NotionWorklifeDatabaseId = GetEnvironmentVariable("NOTION_WORKLIFE_DB_ID");
            MongoUri = GetEnvironmentVariable("MONGO_URI");
            MongoDatabase = GetEnvironmentVariable("MONGO_DB_NAME");
        }

        private string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
