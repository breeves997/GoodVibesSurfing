using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnurfReportService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Text;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class SnowReportController : Controller
    {
        private readonly ISnowReportsService UserReportsSvc;
        public SnowReportController(ISnowReportsService svc)
        {
            UserReportsSvc = svc;
        }
        [HttpGet("{id}")]
        public async Task<SnowReport> Get(long id)
        {
            SnowReport report = await UserReportsSvc.GetSnowReport(id);
            return report;
        }
        [HttpGet]
        public async Task<IEnumerable<SnowReport>> Get(DateTime date, string poster)
        {
            if (String.IsNullOrWhiteSpace(poster))
            {
                IEnumerable<SnowReport> report = await UserReportsSvc.GetDailySnowReports(date.Date);
                return report;
            }
            else
                return await UserReportsSvc.GetDailySnowReportByKey(date.Date, poster);
        }
        [HttpPost]
        public async Task<SnowReport> Post([FromBody] SnowReport report)
        {
            SnowReport rtn = await UserReportsSvc.SaveSnowReport(report);
            //this happens if the given poster has already provided a report for the day
            //TODO: handle this more elegantly
            if (rtn == null)
            {
                Response.StatusCode = 204;
                var jsonString = "{\"message\":\"You have already submitted a report for today.\"}";
                Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await Response.WriteAsync(jsonString, Encoding.UTF8);
                return null;
            }
            return rtn;

        }
        [HttpPut]
        public async Task<SnowReport> Put([FromBody] SnowReport report)
        {
            return await Post(report);
        }
    }

}
