using System;
using System.Linq;
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

        //[FunctionName("ServeDashboardHtml")]
        //public async Task<IActionResult> ServeDashboardHtml([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "/")] HttpRequest req, ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");



        //    return new OkObjectResult(responseMessage);
        //}

        [FunctionName("ServeDashboardData")]
        public async Task<IActionResult> ServeDashboardData([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "data")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new JsonResult(new
            {
                performances = await _domain.GetAllPerformance(),
                statistics = await _domain.GetAllStatistics(),
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
