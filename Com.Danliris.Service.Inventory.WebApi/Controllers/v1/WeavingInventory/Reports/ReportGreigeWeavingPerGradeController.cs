using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerGrade;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory.Reports
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/report-last-stock-greige-grades")]
    [Authorize]
    public class ReportGreigeWeavingPerGradeController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IReportGreigeWeavingPerGradeService service;
        private readonly IServiceProvider serviceProvider;
        private readonly IdentityService identityService;

        public ReportGreigeWeavingPerGradeController(IReportGreigeWeavingPerGradeService service, IServiceProvider serviceProvider)
        {
            this.service = service;
            this.serviceProvider = serviceProvider;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public async Task<IActionResult> GetStock(DateTime? dateTo, int page = 1, int size = 25, string Order = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = await service.GetStockGrade(dateTo, offset, page, size, Order);



                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
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
        public async Task<IActionResult> GetExcelAll([FromHeader(Name = "x-timezone-offset")] string timezone, DateTime? dateTo)
        {
            try
            {
                //VerifyUser();
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(timezone);
                DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;
                string Tanggal = DateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                var Result = await service.GenerateExcel(dateTo, clientTimeZoneOffset);
                string filename = "Saldo Akhir Gudang Grey per Grade- " + Tanggal + ".xlsx";

                xlsInBytes = Result.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
