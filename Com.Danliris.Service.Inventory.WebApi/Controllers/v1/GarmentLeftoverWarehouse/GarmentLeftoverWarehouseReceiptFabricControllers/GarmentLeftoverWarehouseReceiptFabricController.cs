using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricModels;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricViewModels;
using Com.Danliris.Service.Inventory.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Danliris.Service.Inventory.WebApi.Controllers.v1.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment/leftover-warehouse-receipts/fabric")]
    [Authorize]
    public class GarmentLeftoverWarehouseReceiptFabricController : BaseController<GarmentLeftoverWarehouseReceiptFabric, GarmentLeftoverWarehouseReceiptFabricViewModel, IGarmentLeftoverWarehouseReceiptFabricService>
    {
        public GarmentLeftoverWarehouseReceiptFabricController(IIdentityService identityService, IValidateService validateService, IGarmentLeftoverWarehouseReceiptFabricService service) : base(identityService, validateService, service, "1.0.0")
        {
        }
    }
}
