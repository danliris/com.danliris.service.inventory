using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.Net;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventoryWeavingReport
{
    public class InventoryWeavingOutUploadControllerTest
    {
        protected InventoryWeavingUploadCsvOutViewModel ViewModel
        {
            get { return new InventoryWeavingUploadCsvOutViewModel(); }
        }

        protected InventoryWeavingDocument Model
        {
            get { return new InventoryWeavingDocument(); }
        }

        protected InventoryWeavingDocumentOutUploadViewModel viewmodel
        {
            get { return new InventoryWeavingDocumentOutUploadViewModel(); }
        }

        protected ServiceValidationExeption GetServiceValidationException()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryWeavingDocumentOutService> service, Mock<IMapper> mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), service: new Mock<IInventoryWeavingDocumentOutService>(), mapper: new Mock<IMapper>());
        }

        protected WeavingInventoryOutController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryWeavingDocumentOutService> service, Mock<IMapper> mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim ("username", "unittestusername")
            };

            user.Setup(u => u.Claims).Returns(claims);
            WeavingInventoryOutController controller = (WeavingInventoryOutController)Activator.CreateInstance(typeof(WeavingInventoryOutController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.service.Object, mocks.mapper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        public InventoryWeavingDocumentOutUploadViewModel Vm()
        {
            return new InventoryWeavingDocumentOutUploadViewModel
            {

                date = DateTimeOffset.Now,
                bonNo = "test01",
                bonType = "PRODUKSI",
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",

                type = "OUT",
                remark = "Remark",
                itemsOut = new List<InventoryWeavingDocumentOutItemViewModel> { new InventoryWeavingDocumentOutItemViewModel()
                    {
                        ProductOrderNo = "product",
                        ReferenceNo = "referencce",
                        Construction = "CD",
                        Grade = "A",
                        Piece = "1",
                        MaterialName = "CD",
                        WovenType = "",
                        Yarn1 = "yarn1",
                        Yarn2 = "yarn2",
                        YarnType1 = "yt1",
                        YarnType2 = "yt2",
                        YarnOrigin1 = "yo1",
                        YarnOrigin2 = "yo2",
                        Width = "1",
                        UomUnit = "MTR",
                        Quantity = 1,
                        QuantityPiece =1,
                        ProductRemark = "",
                        Barcode = "barcode",
                        ProductionOrderDate = DateTime.Now,
                } }
            };
        }

        private const string URI = "v1/master/upload-garment-currencies";
        private const string currURI = "v1/master/upload-currencies";

        [Fact]
        public void UploadUploadFile_WithoutException_ReturnOK()
        {
            string header = "nota,nm_tujuan,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,jenis,piece,meter";
            string isi = "nota,nm_tujuan,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,1,1,1";

            //---continue
            var mockFacade = new Mock<IInventoryWeavingDocumentOutService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockFacade.Setup(f => f.CsvHeaderUpload).Returns(header.Split(',').ToList());
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingUploadCsvOutViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
            mockFacade.Setup(f => f.MapToViewModel(It.IsAny<List<InventoryWeavingUploadCsvOutViewModel>>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(Vm);

            var MockMapper = new Mock<IMapper>();

            var model = new InventoryWeavingDocument()
            {
                Date = DateTimeOffset.Now,
                BonNo = "test01",
                BonType = "weaving",
                StorageCode = "test01",
                StorageId = 2,  
                StorageName = "Test",

                Type = "OUT",
                Remark = "Remark",
                Items = new List<InventoryWeavingDocumentItem>()
                {
                    new InventoryWeavingDocumentItem()
                    {
                        ProductOrderName = "product",
                        ReferenceNo = "referencce",
                        Construction = "CD",
                        Grade = "A",
                        Piece = "1",
                        MaterialName = "CD",
                        WovenType = "",
                        Yarn1 = "yarn1",
                        Yarn2 = "yarn2",
                        YarnType1 = "yt1",
                        YarnType2 = "yt2",
                        YarnOrigin1 = "yo1",
                        YarnOrigin2 = "yo2",
                        Width = "1",
                        UomUnit = "MTR",
                        UomId = 1,
                        Quantity = 1,
                        QuantityPiece =1,
                        ProductRemark = "",
                        InventoryWeavingDocumentId = 1,
                        Barcode = "barcode",
                        ProductionOrderDate = DateTime.Now,
                    }
                }
            };

            MockMapper.Setup(x => x.Map<List<InventoryWeavingDocument>>(It.IsAny<List<InventoryWeavingUploadCsvOutViewModel>>())).Returns(new List<InventoryWeavingDocument>() { model });

            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + isi)), 0, Encoding.UTF8.GetBytes(header + "\n" + isi).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.NotNull(response.Result);
        }

        [Fact]
        public void UploadFile_WithException_ReturnError()
        {
            var mockFacade = new Mock<IInventoryWeavingDocumentOutService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Throws(new Exception());

            var mockMapper = new Mock<IMapper>();
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";

            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));

        }

        [Fact]
        public void UploadFile_WithException_FileNotFound()
        {
            string header = "Konstruksi,Benang,Anyaman,Lusi,Pakan,Lebar,JL,JP,AL,AP,Grade,Piece,Qty,QtyPiece,Barcode,ProductionOrderDate";

            var mockFacade = new Mock<IInventoryWeavingDocumentOutService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockFacade.Setup(f => f.CsvHeaderUpload).Returns(header.Split(',').ToList());
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingUploadCsvOutViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));

            var mockMapper = new Mock<IMapper>();
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { });

            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_CSVError()
        {
            string header = "bon,Benang,Anyaman,Lusi,Pakan,Lebar,JL,JP,AL,AP,Grade,Piece,Qty,QtyPiece";
            
            var mockFacade = new Mock<IInventoryWeavingDocumentOutService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeaderUpload).Returns(header.Split(';').ToList());
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingUploadCsvOutViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));

            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_ErrorInFile()
        {
            string header = "Konstruksi,Benang,Anyaman,Lusi,Pakan,Lebar,JL,JP,AL,AP,Grade,Piece,Qty,QtyPiece,Barcode,ProductionOrderDate";
            string isi = "Konstruksi,Benang,Anyaman,Lusi,Pakan,Lebar,JL,JP,AL,AP,Grade,1,1,1,barcode,01/01/2020";

            var mockFacade = new Mock<IInventoryWeavingDocumentOutService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeaderUpload).Returns(header.Split(',').ToList());
            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingUploadCsvOutViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));

            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + isi)), 0, Encoding.UTF8.GetBytes(header + "\n" + isi).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.NotNull(response.Result);
        }
        /*
        private async Task<int> GetStatusCodePost((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryWeavingDocumentOutService> service, Mock<IMapper> mapper) mocks, int id, InventoryWeavingDocumentOutItemViewModel viewModel)
        {
            WeavingInventoryOutController controller = GetController(mocks);
            IActionResult response = await controller.Post(id, viewModel);
        }*/

        [Fact]
        public void GetCsv_Success_PostUpload()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.MapToModelUpload(It.IsAny<InventoryWeavingDocumentOutUploadViewModel>()));
            var controller = GetController(mocks);
            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetCsv_Fail_PostUpload()
        {
            var mocks = GetMocks();
            mocks.service.Setup(f => f.MapToModelUpload(It.IsAny<InventoryWeavingDocumentOutUploadViewModel>()));
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.NotNull(response);
        }

        [Fact]
        public void GetCsv_ValidatePost()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(f => f.Validate(It.IsAny<InventoryWeavingDocumentOutUploadViewModel>()));
            mocks.service.Setup(f =>f.MapToModelUpload(It.IsAny<InventoryWeavingDocumentOutUploadViewModel>())).ReturnsAsync(Model);
            var controller = GetController(mocks);
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";
            var response = controller.postCsvFileAsync(DateTime.Now);
            Assert.NotNull(response);
        }



    }
}
