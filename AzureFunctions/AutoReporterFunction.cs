using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FahmiNotionAutomation.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FahmiNotionAutomation
{
    public class AutoReporterFunction
    {
        private readonly IAutoReporterDomain _domain;

        public AutoReporterFunction(IAutoReporterDomain domain)
        {
            _domain = domain;
        }

        [FunctionName("ServeDashboardHtml")]
        public async Task<IActionResult> ServeDashboardHtml([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboard")] HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("Dashboard page requested");

            var htmlPath = Path.Combine(context.FunctionAppDirectory, "graph.html");
            log.LogInformation(htmlPath);
                        
            log.LogInformation("Dashboard page loaded");
            return new ContentResult()
            {
                Content = File.ReadAllText(htmlPath),
                ContentType = "text/html",
            };
        }

        [FunctionName("ServeDashboardData")]
        public async Task<IActionResult> ServeDashboardData([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "data")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Dashboard data requested");

            var performances = await _domain.GetAllPerformance();
            var performanceChartData = new List<ChartPointDto>();
            if (performances != null)
            {
                performanceChartData = new List<ChartPointDto>
                {
                    new ChartPointDto
                    {
                        Type = "scatter",
                        Name = "2-Day MA",
                        XData = performances.Select(x => x.CurrentPeriod).ToList(),
                        YData = performances.Select(x => x.Commitment).ToList(),
                    },
                    new ChartPointDto
                    {
                        Type = "bar",
                        Name = "Commitment",
                        XData = performances.Select(x => x.CurrentPeriod).ToList(),
                        YData = performances.Select(x => Convert.ToDouble(x.CommitmentChange)).ToList(),
                    },
                };
            }

            var statistics = await _domain.GetAllStatistics();
            var statisticsChartData = new List<ChartPointDto>();
            var latestStatisticsChartData = new List<PieChartPointDto>();
            if (statistics != null)
            {
                statisticsChartData = new List<ChartPointDto>
                {
                    new ChartPointDto
                    {
                        Type = "scatter",
                        Name = "Todo",
                        XData = statistics.Select(x => x.Period).ToList(),
                        YData = statistics.Select(x => Convert.ToDouble(x.Todo)).ToList(),
                    },
                    new ChartPointDto
                    {
                        Type = "scatter",
                        Name = "In progress",
                        XData = statistics.Select(x => x.Period).ToList(),
                        YData = statistics.Select(x => Convert.ToDouble(x.InProgress + x.Review)).ToList(),
                    },
                    new ChartPointDto
                    {
                        Type = "scatter",
                        Name = "Completed",
                        XData = statistics.Select(x => x.Period).ToList(),
                        YData = statistics.Select(x => Convert.ToDouble(x.Completed)).ToList(),
                    },
                };

                var latestStatistics = statistics.Last();
                latestStatisticsChartData = new List<PieChartPointDto>
                {
                    new PieChartPointDto
                    {
                        Type = "pie",
                        Labels = new List<string> {
                            "Backlog",
                            "Triage",
                            "Todo",
                            "InProgress",
                            "Review",
                            "Completed",
                        },
                        Values = new List<double> {
                            latestStatistics.Backlog,
                            latestStatistics.Triage,
                            latestStatistics.Todo,
                            latestStatistics.InProgress,
                            latestStatistics.Review,
                            latestStatistics.Completed
                        }
                    }
                };
            }

            log.LogInformation("Dashboard data processed");
            return new JsonResult(new
            {
                performances = performanceChartData,
                statistics = statisticsChartData,
                latestStatistics = latestStatisticsChartData
            });
        }

        [FunctionName("CalculatePerformance")]
        public async Task CalculatePerformance([TimerTrigger("0 0 0 1 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Notion automation function started at: {DateTime.Now}");

            await _domain.ReportAsync();

            log.LogInformation($"Notion automation function executed at: {DateTime.Now}");
        }

    }
}
