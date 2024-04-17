using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Stock;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using General = Com.Danliris.Service.Inventory.WebApi.Helpers.General;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.Report
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-stocks/report")]
    [Authorize]
    public class GarmentLeftoverWarehouseStockReportController : Controller
    {
        protected IIdentityService IdentityService;
        protected readonly IValidateService ValidateService;
        protected readonly IGarmentLeftoverWarehouseStockReportService Service;
        protected const string ApiVersion = "1.0.0";

        public GarmentLeftoverWarehouseStockReportController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseStockReportService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
        }

        protected void VerifyUser()
        {
            IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            IdentityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet("fabric")]
        public IActionResult GetReportStockFabric(DateTime? dateFrom, DateTime? dateTo, int unit, int page, int size, string Order = "{}")
        {
            try
            {

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                if (page == 0)
                {
                    page = 1;
                    size = 25;
                }
                var data = Service.GetMonitoringFabric(dateFrom, dateTo, unit, page, size, Order, offset);

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

        [HttpGet("fabric-ForMutationBB")]
        public IActionResult GetReportStockFabricForBBCentral(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                //if (page == 0)
                //{
                //    page = 1;
                //    size = 25;
                //}
                var data = Service.GetReportQueryForMutationBB(dateFrom, dateTo, offset);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data,
                    //info = new { total = data.Item2 },
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

        [HttpGet("download-fabric")]
        public IActionResult GetXlStockFabric(DateTime? dateFrom, DateTime? dateTo, int unit, string receiptType)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = Service.GenerateExcelFabric(dateFrom, dateTo, unit, offset);

                string filename = String.Format("Report Stock Gudang Sisa - FABRIC {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
        [HttpGet("acc")]
        public IActionResult GetReportStockAcc(DateTime? dateFrom, DateTime? dateTo, int unit, int page, int size, string Order = "{}")
        {
            try
            {

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                if (page == 0)
                {
                    page = 1;
                    size = 25;
                }
                var data = Service.GetMonitoringAcc(dateFrom, dateTo, unit, page, size, Order, offset);

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

        [HttpGet("download-acc")]
        public IActionResult GetXlsStockAcc(DateTime? dateFrom, DateTime? dateTo, int unit, string receiptType)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = Service.GenerateExcelAcc(dateFrom, dateTo, unit, offset);

                string filename = String.Format("Report Stock Gudang Sisa - Accessories {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
        [HttpGet("finishedgood")]
        public IActionResult GetReportStockFinishedGood(DateTime? dateFrom, DateTime? dateTo, int unit, int page, int size, string ro,string comodityCode, string Order = "{}")
        {
            try
            {
                VerifyUser();
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                if (page == 0)
                {
                    page = 1;
                    size = 25;
                }
                var data = Service.GetMonitoringFinishedGood(dateFrom, dateTo, unit, page, size, Order, offset, ro, comodityCode);

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

        [HttpGet("download-finishedgood")]
        public IActionResult GetXlsStockFinishedGood(DateTime? dateFrom, DateTime? dateTo, int unit, string receiptType, string ro, string comodityCode)
        {

            try
            {
                VerifyUser();
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = Service.GenerateExcelFinishedGood(dateFrom, dateTo, unit, offset,ro,comodityCode);

                string filename = String.Format("Report Stock Gudang Sisa - Barang Jadi {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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

        [HttpGet("aval")]
        public IActionResult GetReportStockAval(DateTime? dateFrom, DateTime? dateTo, int unit, string typeAval, int page, int size, string Order = "{}")
        {
            try
            {

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                if (page == 0)
                {
                    page = 1;
                    size = 25;
                }
                var data = Service.GetMonitoringAval(dateFrom, dateTo, unit, page, size, Order, offset, typeAval);

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

        [HttpGet("download-avals")]
        public IActionResult GetXlsStockAval(DateTime? dateFrom, DateTime? dateTo, int unit, string typeAval)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = Service.GenerateExcelAval(dateFrom, dateTo, unit, offset, typeAval);

                string filename = String.Format("Report Stock Gudang Sisa - Aval {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
