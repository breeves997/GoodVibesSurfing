﻿using System;
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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class SurfReportController : Controller
    {
        private readonly ISurfReportsService UserReportsSvc;
        public SurfReportController(ISurfReportsService svc)
        {
            UserReportsSvc = svc;
        }
        [HttpGet("{id}")]
        public async Task<SurfReport> Get(long id)
        {
            SurfReport report = await UserReportsSvc.GetSurfReport(id);
            return report;
        }
        [HttpGet]
        public async Task<IEnumerable<SurfReport>> Get(DateTime date)
        {
            IEnumerable<SurfReport> report = await UserReportsSvc.GetDailySurfReports(date.Date);
            return report;
        }
        [HttpPost]
        public async Task<SurfReport> Post([FromBody] SurfReport report)
        {
            SurfReport rtn = await UserReportsSvc.SaveSurfReport(report);
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
        public async Task<SurfReport> Put([FromBody] SurfReport report)
        {
            return await Post(report);
        }
    }

}