using AutoMapper;
using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Interfaces.MaterialRequestNote;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialsRequestNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1.BasicControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Controllers.MaterialRequestNote
{
    
    public class MaterialRequestNoteBasicTest
    {
        private MaterialsRequestNoteViewModel ViewModel
        {
            get
            {
                return new MaterialsRequestNoteViewModel
                {
                    Unit = new Lib.ViewModels.UnitViewModel
                    {
                        Id=1,
                        Code = "TEST",
                        Name = "TEST"
                    },
                    RequestType = "AWAL",
                    Remark = "",
                    Code = "Test",
                    IsDistributed = false,
                    IsCompleted = false,
                    MaterialsRequestNote_Items = new List<MaterialsRequestNote_ItemViewModel>
                    {
                        new MaterialsRequestNote_ItemViewModel
                        {
                            ProductionOrder = new Lib.ViewModels.ProductionOrderSQLViewModel
                            {
                                IsCompleted = false,
                                OrderQuantity = 1,
                            },
                            ProductionOrderIsCompleted = false
                        }
                    }
                };
            }
        }

        private MaterialsRequestNote Model
        {
            get
            {
                return new MaterialsRequestNote { };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ViewModel.IsDistributed = ViewModel.IsDistributed;
            ViewModel.IsCompleted = ViewModel.IsCompleted;
            foreach (var item in ViewModel.MaterialsRequestNote_Items)
            {
                item.DistributedLength = item.DistributedLength;
                item.ProductionOrderId = item.ProductionOrderId;
            }
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService2());

            return serviceProvider;
        }

        private MaterialsRequestNoteController GetController(Mock<IMaterialsRequestNote> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateM.Object);
            }

            var controller = new MaterialsRequestNoteController(servicePMock.Object, mapper.Object, facadeM.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialsRequestNote>(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialsRequestNote>()))
               .ReturnsAsync(1);
            mockFacade.Setup(x => x.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(new MaterialsRequestNote());

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IMaterialsRequestNote>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data_Empty()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialsRequestNote>(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialsRequestNote>()))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IMaterialsRequestNote>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialsRequestNote>(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialsRequestNote>()))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IMaterialsRequestNote>();

            MaterialsRequestNoteController controller = new MaterialsRequestNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialsRequestNote>();

            mockFacade.Setup(x => x.ReadModel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<MaterialsRequestNote>(), 0, new Dictionary<string, string>(),new List<string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MaterialsRequestNoteViewModel>>(It.IsAny<List<MaterialsRequestNote>>()))
                .Returns(new List<MaterialsRequestNoteViewModel> { ViewModel });

            MaterialsRequestNoteController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User_With_Filter()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialsRequestNote>();

            mockFacade.Setup(x => x.ReadModel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<MaterialsRequestNote>(), 0, new Dictionary<string, string>(), new List<string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MaterialsRequestNoteViewModel>>(It.IsAny<List<MaterialsRequestNote>>()))
                .Returns(new List<MaterialsRequestNoteViewModel> { ViewModel });

            MaterialsRequestNoteController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(Filter: "{ 'IsDistributed': false }");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialsRequestNote>();

            mockFacade.Setup(x => x.ReadModel(0,0, It.IsAny<string>(), null, null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<MaterialsRequestNote>(), 0, new Dictionary<string, string>(), new List<string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MaterialsRequestNoteViewModel>>(It.IsAny<List<MaterialsRequestNote>>()))
                .Returns(new List<MaterialsRequestNoteViewModel> { ViewModel });

            MaterialsRequestNoteController controller = new MaterialsRequestNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialsRequestNote>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .ReturnsAsync(new MaterialsRequestNote());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialsRequestNoteViewModel>(It.IsAny<MaterialsRequestNote>()))
                .Returns(ViewModel);
            mockFacade.Setup(x => x.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(new MaterialsRequestNote());

            MaterialsRequestNoteController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialsRequestNote>(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.UpdateModel(It.IsAny<int>(), It.IsAny<MaterialsRequestNote>()))
               .ReturnsAsync(1);
            mockFacade.Setup(x => x.MapToModel(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(new MaterialsRequestNote());

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<MaterialsRequestNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IMaterialsRequestNote>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<MaterialsRequestNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialsRequestNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialsRequestNote>(It.IsAny<MaterialsRequestNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialsRequestNote>()))
               .ReturnsAsync(1);

            var controller = new MaterialsRequestNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Put(It.IsAny<int>(), It.IsAny<MaterialsRequestNoteViewModel>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.DeleteModel(It.IsAny<int>()))
                .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public async void Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IMaterialsRequestNote>();
            mockFacade.Setup(x => x.DeleteModel(It.IsAny<int>()))
                .ReturnsAsync(1);

            var controller = new MaterialsRequestNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
