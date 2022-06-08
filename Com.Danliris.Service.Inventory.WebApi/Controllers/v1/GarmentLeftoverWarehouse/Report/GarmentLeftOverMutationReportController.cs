using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Mutation;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.AvalMutation;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/customs-reports/scrap")]
    [Authorize]
    public class GarmentLeftOverMutationReportController : Controller
    {
        protected IIdentityService IdentityService;
        protected readonly IValidateService ValidateService;
        protected readonly IGarmentLeftoverWarehouseMutationReportService Service;
        protected readonly IGarmentLeftoverWarehouseAvalMutationReportService avalMutationService;
        protected const string ApiVersion = "1.0.0";

        public GarmentLeftOverMutationReportController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseMutationReportService service, IGarmentLeftoverWarehouseAvalMutationReportService garmentLeftoverWarehouseAvalMutationReportService)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            avalMutationService = garmentLeftoverWarehouseAvalMutationReportService;
        }

        [HttpGet("mutation")]
        public IActionResult GetMutation(DateTime? dateFrom, DateTime? dateTo, int page = 1, int size = 25)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = Service.GetMutation(dateFrom, dateTo,page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
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

        [HttpGet("mutation/download")]
        public IActionResult GetXlsScrap(DateTime dateFrom, DateTime dateTo)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = Service.GenerateExcelMutation(dateFrom, dateTo);

                string filename = String.Format("Laporan Pertanggungjawaban Barang Reject dan Scrap - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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

        [HttpGet("aval-besar-in")]
        public IActionResult GetAvalBesar(DateTime? dateFrom, DateTime? dateTo, int page = 1, int size = 25)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = avalMutationService.GetAvalBesarIn(dateFrom, dateTo, page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
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

        [HttpGet("aval-besar-in/download")]
        public IActionResult GetXlsAvalBesar(DateTime dateFrom, DateTime dateTo)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = avalMutationService.GenerateExcelAvalBesar(dateFrom, dateTo);

                string filename = "Laporan Monitoring Pemasukan Aval Besar";

                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");

                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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

        [HttpGet("aval-besar-out")]
        public IActionResult GetAvalBesarOut(DateTime? dateFrom, DateTime? dateTo, int page = 1, int size = 25)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = avalMutationService.GetAvalBesarOut(dateFrom, dateTo, page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
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

        [HttpGet("aval-besar-out/download")]
        public IActionResult GetXlsAvalBesarOut(DateTime dateFrom, DateTime dateTo)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = avalMutationService.GenerateExcelAvalBesarOut(dateFrom, dateTo);

                string filename = "Laporan Monitoring Pengeluaran Aval Besar";

                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");

                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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

        [HttpGet("aval-komponen-in")]
        public IActionResult GetAvalKomponenIn(DateTime? dateFrom, DateTime? dateTo, int page = 1, int size = 25)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = avalMutationService.GetAvalKomponenIn(dateFrom, dateTo, page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
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

        [HttpGet("aval-komponen-in/download")]
        public IActionResult GetXlsAvalKomponen(DateTime dateFrom, DateTime dateTo)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = avalMutationService.GenerateExcelAvalKomponenIn(dateFrom, dateTo);

                string filename = "Laporan Monitoring Pemasukan Aval Komponen";

                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");

                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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

        [HttpGet("aval-komponen-out")]
        public IActionResult GetAvalKomponenOut(DateTime? dateFrom, DateTime? dateTo, int page = 1, int size = 25)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var data = avalMutationService.GetAvalKomponenOut(dateFrom, dateTo, page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
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
        [HttpGet("aval-komponen-out/download")]
        public IActionResult GetXlsAvalKomponenOut(DateTime dateFrom, DateTime dateTo)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.TimezoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                byte[] xlsInBytes;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = avalMutationService.GenerateExcelAvalKomponenOut(dateFrom, dateTo);

                string filename = "Laporan Monitoring Pengeluaran Aval Komponen";

                if (dateFrom != null) filename += " " + ((DateTime)dateFrom).ToString("dd-MM-yyyy");

                if (dateTo != null) filename += "_" + ((DateTime)dateTo).ToString("dd-MM-yyyy");
                filename += ".xlsx";

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
