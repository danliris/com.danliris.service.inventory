using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.InventoryWeavingDocumentOutService;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/output-inventory-weaving")]
    [Authorize]
    public class WeavingInventoryOutController : Controller
    {
        protected IIdentityService IdentityService;
        protected readonly IMapper Mapper;
        protected readonly IValidateService ValidateService;
        protected readonly IInventoryWeavingDocumentOutService Service;
        protected readonly string ApiVersion;
        private readonly string ContentType = "application/vnd.openxmlformats";
        private readonly string FileName = string.Concat("Error Log - ", typeof(InventoryWeavingDocument).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");

        public WeavingInventoryOutController(IIdentityService identityService, IValidateService validateService, IInventoryWeavingDocumentOutService service, IMapper mapper)
        {
            Mapper = mapper;
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = "1.0.0";
        }

        protected void VerifyUser()
        {
            IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            IdentityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string keyword = null, [FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string order = "{}",
            [FromQuery] string filter = "{}")
        {
            try
            {

                var data = Service.Read(page, size, order, keyword, filter);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }



        }

        [HttpGet("material-loader")]
        public IActionResult GetDistinctMaterial([FromQuery] string keyword = null, [FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string order = "{}",
            [FromQuery] string filter = "{}")
        {
            try
            {

                var data = Service.GetDistinctMaterial(page, size, filter, order, keyword);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpGet("output-material")]
        public IActionResult GetMaterial([FromQuery] string material)
        {
            try
            {

                var data = Service.GetMaterialItemList(material);
                return Ok(new
                {
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]InventoryWeavingDocumentOutViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);

                InventoryWeavingDocument model = await Service.MapToModel(viewModel);


                await Service.Create(model);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }


        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {

                var data = Service.ReadById(id);
                return Ok(new
                {
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("download")]
        public IActionResult DownloadTemplate( DateTime dateFrom, DateTime dateTo, [FromHeader(Name = "x-timezone-offset")] string timezone, string bonType)
        {
            try
            {
                byte[] csvInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(timezone);
                var csv = Service.DownloadCSVOut( dateFrom, dateTo, clientTimeZoneOffset, bonType);

                //string fileName = "Chart of Account Template.csv";
                string fileName = "Export CSV To area" + bonType + " From Date " + dateFrom.ToString("MM/dd/yyyy") + " - " + dateTo.ToString("MM/dd/yyyy")+".csv";

                csvInBytes = csv.ToArray();

                var file = File(csvInBytes, "text/csv", fileName);
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


        [HttpGet("report")]
        public IActionResult GetReport(string destination, DateTime? dateFrom, DateTime? dateTo,  int page = 1, int size = 25, string Order = "{}")
        {
            try
            {
                VerifyUser();

                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var data = Service.GetReport(destination, dateFrom, dateTo, page, size, Order, offset);

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

        [HttpGet("report/download")]
        public IActionResult GetExcelAll([FromHeader(Name = "x-timezone-offset")] string timezone, string destination, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            try
            {
                //VerifyUser();
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(timezone);
                var Result = Service.GenerateExcelReceiptReport(destination, dateFrom, dateTo, clientTimeZoneOffset);
                string filename = "Monitoring Penerimaan Gudang Weaving.xlsx";

                xlsInBytes = Result.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        [HttpPost("upload-output")]
        public async Task<IActionResult> postCsvFileAsync(DateTime date)
        {
            //public async Task<IActionResult> postCsvFileAsync (string destination, DateTime date)

            try
            {
                //VerifyUser();
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
                IdentityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                if (Request.Form.Files.Count > 0)
                {
                    var UploadedFiles = Request.Form.Files[0];
                    StreamReader Reader = new StreamReader(UploadedFiles.OpenReadStream());
                    List<string> FileHeader = new List<string>(Reader.ReadLine().Split(","));
                    var validHeader = Service.CsvHeaderUpload.SequenceEqual(FileHeader, StringComparer.OrdinalIgnoreCase);

                    if (validHeader)
                    {
                        Reader.DiscardBufferedData();
                        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        Reader.BaseStream.Position = 0;
                        CsvReader Csv = new CsvReader(Reader);
                        Csv.Configuration.IgnoreBlankLines = false;
                        Csv.Configuration.Delimiter = ",";
                        Csv.Configuration.RegisterClassMap<WeavingInventoryOutMap>();
                        Csv.Configuration.HeaderValidated = null;

                        List<InventoryWeavingUploadCsvOutViewModel> Data = Csv.GetRecords<InventoryWeavingUploadCsvOutViewModel>().ToList();
                        InventoryWeavingDocumentOutUploadViewModel Data1 = await Service.MapToViewModel(Data, date);


                        ValidateService.Validate(Data1);

                        Tuple<bool, List<object>> Validated = Service.UploadValidate(ref Data, Request.Form.ToList());

                        Reader.Close();

                        if (Validated.Item1)
                        {

                            List<InventoryWeavingDocumentOutItemViewModel> DataCheck = Data1.itemsOut.ToList();
                            var CheckCsv = Service.checkCsv(DataCheck);


                            if (CheckCsv == 0)
                            {
                                InventoryWeavingDocument data = await Service.MapToModelUpload(Data1);

                                await Service.UploadData(data, IdentityService.Username);

                                Dictionary<string, object> Result =
                                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                                    .Ok();
                                return Created(HttpContext.Request.Path, Result);
                            }
                            else
                            {
                                Dictionary<string, object> Result = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, "Barcode Belum ada di penyimpanan!").Fail();

                                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
                            }
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                                using (CsvWriter csvWriter = new CsvWriter(streamWriter))
                                {
                                    csvWriter.WriteRecords(Validated.Item2);
                                }

                                return File(memoryStream.ToArray(), ContentType, FileName);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, object> Result =
                           new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, General.CSV_ERROR_MESSAGE)
                           .Fail();

                        return NotFound(Result);
                    }
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.NO_FILE_ERROR_MESSAGE)
                            .Fail();
                    return BadRequest(Result);
                }
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
