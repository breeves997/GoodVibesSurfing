using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SnurfReportService.Interfaces;
using CloudUtilities;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Text;
using ValidationService.Interfaces;
using ValidationService.Contracts;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class SurfReportController : Controller
    {
        /// <summary>
        /// A proxy to the stateful service for posting surf reports
        /// </summary>
        private readonly ISurfReportsService UserReportsSvc;
        /// <summary>
        /// A proxy to the stateful service for validating snurf reports
        /// </summary>
        private readonly ISnurfReportValidationService ValidatorSvc;
        public SurfReportController(ISurfReportsService svc, ISnurfReportValidationService validator)
        {
            UserReportsSvc = svc;
            ValidatorSvc = validator;
        }
        [HttpGet("{id}")]
        public async Task<SurfReport> Get(long id)
        {
            SurfReport report = await UserReportsSvc.GetSurfReport(id);
            return report;
        }
        /// <summary>
        /// Have I mentioned I don't 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="poster"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<SurfReport>> Get(DateTime date, string poster)
        {
            if (String.IsNullOrWhiteSpace(poster))
            {
                IEnumerable<SurfReport> report = await UserReportsSvc.GetDailySurfReports(date.Date);
                return report;
            }
            else
                return await UserReportsSvc.GetDailySurfReportByKey(date.Date, poster);
        }
        [HttpPost]
        public async Task<SurfReport> Post([FromBody] SurfReport report)
        {
            ValidationResult vld = await ValidatorSvc.ValidateSnurfReport(report);
            if (!vld.Success)
            {
                Response.StatusCode = 400;
                var jsonString = JsonConvert.SerializeObject(vld);
                Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await Response.WriteAsync(jsonString, Encoding.UTF8);
                return null;
            }
            SurfReport rtn = null;
            if (vld.Success)
            {
                rtn = await UserReportsSvc.SaveSurfReport(report);
            }
            //this happens if the given poster has already provided a report for the day
            //TODO: handle this more elegantly
            if (rtn == null && vld.Success)
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
        public async Task<SurfReport> Put([FromBody] SurfReport report)
        {
            return await Post(report);
        }
    }

}
