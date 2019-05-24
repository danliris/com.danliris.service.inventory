using AutoMapper;
using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Facades.InventoryFacades;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/inventory/inventory-summaries")]
    [Authorize]
    public class InventorySummaryController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly InventorySummaryFacade _facade;
        private readonly IdentityService identityService;
        public InventorySummaryController(IMapper mapper, InventorySummaryFacade facade, IdentityService identityService)
        {
            _mapper = mapper;
            _facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                var Data = _facade.Read(page, size, order, keyword, filter);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = Data.Item1,
                    info = new { total = Data.Item2 },
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                InventorySummary model = _facade.ReadModelById(id);
                //InventoryDocumentViewModel viewModel = _mapper.Map<InventoryDocumentViewModel>(model);
                InventorySummaryViewModel viewModel = new InventorySummaryViewModel
                {
                    code = model.No,
                    productCode = model.ProductCode,
                    productId = model.ProductId,
                    productName = model.ProductName,
                    storageCode = model.StorageCode,
                    storageId = model.StorageId,
                    storageName = model.StorageName,
                    uomId = model.UomId,
                    uom = model.UomUnit,
                    quantity = model.Quantity,
                    stockPlanning = model.StockPlanning.ToString()
                };
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = viewModel,
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



        //[HttpPost]
        //public async Task<ActionResult> Post([FromBody] InventoryDocumentViewModel viewModel)
        //{
        //    this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
        //    this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

        //    ValidateService validateService = (ValidateService)_facade.serviceProvider.GetService(typeof(ValidateService));
        //    return await new BasePost<InventoryDocument, InventoryDocumentViewModel, InventoryDocumentFacade>(_facade, ApiVersion, validateService, Request.Path)
        //        .Post(viewModel);
        //}
    }
}