using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportExpenseGreigeWeaving;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory.Reports.ReportExpenseGreigeWeavingController
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/report-expense-greige-weavings")]
    [Authorize]
    public class ReportExpenseGreigeWeavingController : Controller
    {
        protected IIdentityService IdentityService;
        protected readonly IValidateService ValidateService;
        protected readonly IReportExpenseGreigeWeavingService Service;
        protected readonly string ApiVersion;
        public ReportExpenseGreigeWeavingController(IIdentityService identityService, IValidateService validateService, IReportExpenseGreigeWeavingService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = "1.0.0";
        }


        [HttpGet]
        public IActionResult GetExpense(string bonType, DateTime? dateFrom, DateTime? dateTo, [FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string order = "{}"
          )
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = Service.GetReportExpense(bonType, dateFrom, dateTo, page, size, order, offset);
                return Ok(new
                {
                    Data = data.Item1,
                    TotalData = data.Item2,

                });
                //return Ok(data);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);

            }
        }


        [HttpGet("download")]
        public IActionResult GetXlsAll(string bonType, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
                DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

                var xls = Service.GenerateExcelExpense(bonType, dateFrom, dateTo, offset);

                string filename = String.Format("Rincian Pengeluaran - {0}.xlsx", DateTimeOffset.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
