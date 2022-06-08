using AutoMapper;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.WeavingInventory;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.InventoryWeavingReport
{
    public class InventoryWeavingUploadDocumentControllerTest
    {

        //protected Lib.Models.InventoryWeavingModel.InventoryWeavingMovement Model
        //{
        //    get { return new Lib.Models.InventoryWeavingModel.InventoryWeavingMovement(); }
        //}

        protected InventoryWeavingDocumentCsvViewModel ViewModel
        {
            get { return new InventoryWeavingDocumentCsvViewModel(); }
        }
        protected InventoryWeavingDocument Model
        {
            get { return new InventoryWeavingDocument(); }
        }
        protected InventoryWeavingDocumentDetailViewModel viewModel
        {
            get { return new InventoryWeavingDocumentDetailViewModel(); }
        }


        protected ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryWeavingDocumentUploadService> service, Mock<IMapper>mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), service: new Mock<IInventoryWeavingDocumentUploadService>(), mapper:new Mock<IMapper>());
        }

        protected WeavingInventoryUploadController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryWeavingDocumentUploadService> service, Mock<IMapper> mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            WeavingInventoryUploadController controller = (WeavingInventoryUploadController)Activator.CreateInstance(typeof(WeavingInventoryUploadController), mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.service.Object, mocks.mapper.Object);
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

        private const string URI = "v1/master/upload-garment-currencies";
        private const string currURI = "v1/master/upload-currencies";

        [Fact]
        public void UploadFile_WithoutException_ReturnOK()
        {

            string header = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,jenis,piece,meter";
            string isi = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,1,1,1";
            var mockFacade = new Mock<IInventoryWeavingDocumentUploadService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingDocumentCsvViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
           // COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
           // mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));
            var model = new InventoryWeavingDocument()
            {
                Date = DateTimeOffset.Now,
                BonNo = "test01",
                BonType = "weaving",
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",

                Type = "IN",
                Remark = "Remark",
                Items = new List<InventoryWeavingDocumentItem> { new InventoryWeavingDocumentItem(){
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
                } }

            };
            mockMapper.Setup(x => x.Map<List<InventoryWeavingDocument>>(It.IsAny<List<InventoryWeavingDocumentCsvViewModel>>())).Returns(new List<InventoryWeavingDocument>() { model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + isi)), 0, Encoding.UTF8.GetBytes(header + "\n" + isi).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync("PRODUKSI", DateTime.Now);
            Assert.NotNull(response.Result);
        }


        [Fact]
        public void UploadFile_WithException_ReturnError()
        {
            var mockFacade = new Mock<IInventoryWeavingDocumentUploadService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            var mockIdentityService = new Mock<IIdentityService>();

            var mockValidateService = new Mock<IValidateService>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapper));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";

            var response = controller.PostCSVFileAsync("", DateTime.Now);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
        }


        [Fact]
        public void UploadFile_WithException_FileNotFound()
        {

            string header = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,jenis,piece,panjang";
            //string isi = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,1,1,1";
            var mockFacade = new Mock<IInventoryWeavingDocumentUploadService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingDocumentCsvViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(true, new List<object>()));
            // COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            // mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));
            //var model = new InventoryWeavingDocument()
            //{

            //};
           // mockMapper.Setup(x => x.Map<List<InventoryWeavingDocument>>(It.IsAny<List<InventoryWeavingDocumentCsvViewModel>>())).Returns(new List<InventoryWeavingDocument>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { });

            var response = controller.PostCSVFileAsync("PRODUKSI", DateTime.Now);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_CSVError()
        {
            string header = "bon,benang,type,lusi,pakan,lebar,benang,jpakan,alusi,benang,sp,grade,jenis,jumlah,panjang benang";
            //string isi = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,1,1,1";
            var mockFacade = new Mock<IInventoryWeavingDocumentUploadService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(';').ToList());

            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingDocumentCsvViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));
            // COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            // mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));
            //var model = new InventoryWeavingDocument()
            //{

            //};
            // mockMapper.Setup(x => x.Map<List<InventoryWeavingDocument>>(It.IsAny<List<InventoryWeavingDocumentCsvViewModel>>())).Returns(new List<InventoryWeavingDocument>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + header)), 0, Encoding.UTF8.GetBytes(header + "\n" + header).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync("", DateTime.Now);
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response.Result));
        }

        [Fact]
        public void UploadFile_WithException_ErrorInFile()
        {

            string header = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,jenis,piece,meter";
            string isi = "nota,benang,type,lusi,pakan,lebar,jlusi,jpakan,alusi,apakan,sp,grade,1,1,1";
            var mockFacade = new Mock<IInventoryWeavingDocumentUploadService>();
            mockFacade.Setup(f => f.UploadData(It.IsAny<InventoryWeavingDocument>(), It.IsAny<string>())).Verifiable();
            mockFacade.Setup(f => f.CsvHeader).Returns(header.Split(',').ToList());

            mockFacade.Setup(f => f.UploadValidate(ref It.Ref<List<InventoryWeavingDocumentCsvViewModel>>.IsAny, It.IsAny<List<KeyValuePair<string, StringValues>>>())).Returns(new Tuple<bool, List<object>>(false, new List<object>()));
            // COAProfile profile = new COAProfile();

            var mockMapper = new Mock<IMapper>();
            // mockMapper.Setup(x => x.ConfigurationProvider).Returns(new MapperConfiguration(cfg => cfg.AddProfile(profile)));
            //var model = new InventoryWeavingDocument()
            //{

            //};
            // mockMapper.Setup(x => x.Map<List<InventoryWeavingDocument>>(It.IsAny<List<InventoryWeavingDocumentCsvViewModel>>())).Returns(new List<InventoryWeavingDocument>() { Model });
            var mockIdentityService = new Mock<IIdentityService>();
            var mockValidateService = new Mock<IValidateService>();
            var mockMapperService = new Mock<IMapper>();

            var controller = GetController((mockIdentityService, mockValidateService, mockFacade, mockMapperService));
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = $"{It.IsAny<int>()}";
            controller.ControllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(header + "\n" + isi)), 0, Encoding.UTF8.GetBytes(header + "\n" + isi).LongLength, "Data", "test.csv");
            controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });

            var response = controller.PostCSVFileAsync("", DateTime.Now);
            Assert.NotNull(response.Result);
        }

        private async Task<int> GetStatusCodePut((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IInventoryWeavingDocumentUploadService> Service, Mock<IMapper> Mapper) mocks, int id, InventoryWeavingDocumentDetailViewModel viewModel)
        {

            WeavingInventoryUploadController controller = GetController(mocks);
            IActionResult response = await controller.Put(id, viewModel);

            return GetStatusCode(response);
        }

        [Fact]
        public virtual async Task Put_ValidId_ReturnNoContent()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).Verifiable();
            mocks.service.Setup(f => f.MapToModelUpdate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).ReturnsAsync(Model);
            var id = 1;
            var viewModel = new InventoryWeavingDocumentDetailViewModel()
            {
                Id = id
            };
            mocks.service.Setup(f => f.UpdateAsync(It.IsAny<int>(), It.IsAny<InventoryWeavingDocument>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodePut(mocks, id, viewModel);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public virtual async Task Put_InvalidId_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).Verifiable();
            mocks.service.Setup(f => f.MapToModelUpdate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).ReturnsAsync(Model);
            var id = 1;
            var viewModel = new InventoryWeavingDocumentDetailViewModel()
            {
                Id = id + 1
            };

            int statusCode = await GetStatusCodePut(mocks, id, viewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public virtual async Task Put_ThrowServiceValidationExeption_ReturnBadRequest()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).Throws(GetServiceValidationExeption());
            mocks.service.Setup(f => f.MapToModelUpdate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).ReturnsAsync(Model);
            int statusCode = await GetStatusCodePut(mocks, 1, viewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public virtual async Task Put_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(vs => vs.Validate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).Verifiable();
            var id = 1;
            var viewModel = new InventoryWeavingDocumentDetailViewModel()
            {
                Id = id
            };
            mocks.service.Setup(f => f.UpdateAsync(It.IsAny<int>(), It.IsAny<InventoryWeavingDocument>())).ThrowsAsync(new Exception());
            mocks.service.Setup(f => f.MapToModelUpdate(It.IsAny<InventoryWeavingDocumentDetailViewModel>())).ReturnsAsync(Model);
            int statusCode = await GetStatusCodePut(mocks, id, viewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

    }
}
