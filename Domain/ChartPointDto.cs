using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FahmiNotionAutomation.Domain
{
    public record PieChartPointDto
    {
        [JsonProperty("values")]
        public IList<double> Values { get; set; }

        [JsonProperty("labels")]
        public IList<string> Labels { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public record ChartPointDto
    {
        [JsonProperty("x")]
        public IList<DateTime> XData { get; set; }

        [JsonProperty("y")]
        public IList<double> YData { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
