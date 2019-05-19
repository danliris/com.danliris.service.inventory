using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services.MaterialDistributionNoteService;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Model = Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Com.Danliris.Service.Inventory.Lib.ViewModels.MaterialDistributionNoteViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using Com.Moonlay.NetCore.Lib.Service;
using Moq;
using System.ComponentModel.DataAnnotations;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.WebApi.Controllers.v1;
using Com.Danliris.Service.Inventory.Lib.Interfaces.MaterialDistributionNotes;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net;

namespace Com.Danliris.Service.Inventory.Test.Controllers.MaterialDistributionNotes
{
    
    public class MaterialDistributionNoteBasicTest
    {
        private MaterialDistributionNoteViewModel ViewModel
        {
            get
            {
                return new MaterialDistributionNoteViewModel
                {
                    Unit = new Lib.ViewModels.UnitViewModel
                    {
                        Id = 1,
                        _id = "1",
                        code = "TEST",
                        name = "TEST"
                    },
                    Type = "PRODUKSI",
                    IsApproved = false,
                    IsDisposition = false,
                    MaterialDistributionNoteItems = new List<MaterialDistributionNoteItemViewModel>
                    {
                        new MaterialDistributionNoteItemViewModel
                        {
                            MaterialDistributionNoteId = 1,
                            MaterialRequestNoteId = 1,
                            MaterialRequestNoteCode = "TEST",
                            MaterialRequestNoteCreatedDateUtc = new DateTime(),
                            MaterialDistributionNoteDetails = new List<MaterialDistributionNoteDetailViewModel>
                            {
                                new MaterialDistributionNoteDetailViewModel
                                {
                                    MaterialDistributionNoteItemId = 1,
                                    MaterialsRequestNoteItemId = 1,
                                    ProductionOrder = new Lib.ViewModels.ProductionOrderViewModel
                                    {
                                        _id = "1",
                                        orderNo = "1",
                                        orderQuantity= 1,
                                        isCompleted = false,
                                        distributedQuantity = 1,
                                        orderType = new Lib.ViewModels.OrderTypeViewModel
                                        {
                                            _id = "1",
                                            code = "TEST",
                                            name = "TEST"
                                        },
                                    },
                                    Product = new Lib.ViewModels.ProductViewModel
                                    {
                                        _id = "1",
                                        code = "TEST",
                                        name = "TEST"
                                    },
                                    Grade = "",
                                    Quantity = 1,
                                    MaterialRequestNoteItemLength = 1,
                                    DistributedLength = 1,
                                    ReceivedLength = 1,
                                    IsDisposition = false,
                                    IsCompleted = false,
                                    Supplier = new Lib.ViewModels.SupplierViewModel
                                    {
                                        _id = "1",
                                        code = "TEST",
                                        name = "TEST"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        private MaterialDistributionNote Model
        {
            get
            {
                return new MaterialDistributionNote { };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ViewModel.IsApproved = ViewModel.IsApproved;
            ViewModel.IsDisposition = ViewModel.IsDisposition;
            foreach (var item in ViewModel.MaterialDistributionNoteItems)
            {
                item.MaterialDistributionNoteId = item.MaterialDistributionNoteId;
                item.MaterialRequestNoteId = item.MaterialRequestNoteId;
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

        private MaterialDistributionNoteController GetController(Mock<IMaterialDistributionNote> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            var controller = new MaterialDistributionNoteController(servicePMock.Object, mapper.Object, facadeM.Object)
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
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialDistributionNote>(It.IsAny<MaterialDistributionNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialDistributionNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialDistributionNote>()))
               .ReturnsAsync(1);
            mockFacade.Setup(x => x.MapToModel(It.IsAny<MaterialDistributionNoteViewModel>()))
                .Returns(new MaterialDistributionNote());

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IMaterialDistributionNote>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data_Empty()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialDistributionNote>(It.IsAny<MaterialDistributionNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialDistributionNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialDistributionNote>()))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialDistributionNote>(It.IsAny<MaterialDistributionNoteViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<IMaterialDistributionNote>();
            mockFacade.Setup(x => x.CreateModel(It.IsAny<MaterialDistributionNote>()))
               .ReturnsAsync(1);

            var IPOmockFacade = new Mock<IMaterialDistributionNote>();

            MaterialDistributionNoteController controller = new MaterialDistributionNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialDistributionNote>();

            mockFacade.Setup(x => x.ReadModel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<MaterialDistributionNote>(), 0, new Dictionary<string, string>(), new List<string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MaterialDistributionNoteViewModel>>(It.IsAny<List<MaterialDistributionNote>>()))
                .Returns(new List<MaterialDistributionNoteViewModel> { ViewModel });

            MaterialDistributionNoteController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User_With_Filter()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialDistributionNote>();

            mockFacade.Setup(x => x.ReadModel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<MaterialDistributionNote>(), 0, new Dictionary<string, string>(), new List<string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MaterialDistributionNoteViewModel>>(It.IsAny<List<MaterialDistributionNote>>()))
                .Returns(new List<MaterialDistributionNoteViewModel> { ViewModel });

            MaterialDistributionNoteController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(Filter: "{ 'IsApproved': false }");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialDistributionNote>();

            mockFacade.Setup(x => x.ReadModel(0, 0, It.IsAny<string>(), null, null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<MaterialDistributionNote>(), 0, new Dictionary<string, string>(), new List<string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<MaterialDistributionNoteViewModel>>(It.IsAny<List<MaterialDistributionNote>>()))
                .Returns(new List<MaterialDistributionNoteViewModel> { ViewModel });

            MaterialDistributionNoteController controller = new MaterialDistributionNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async void Should_Success_Get_Data_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<MaterialDistributionNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IMaterialDistributionNote>();

            mockFacade.Setup(x => x.ReadModelById(It.IsAny<int>()))
                .ReturnsAsync(new MaterialDistributionNote());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<MaterialDistributionNoteViewModel>(It.IsAny<MaterialDistributionNote>()))
                .Returns(ViewModel);
            mockFacade.Setup(x => x.MapToModel(It.IsAny<MaterialDistributionNoteViewModel>()))
                .Returns(new MaterialDistributionNote());

            MaterialDistributionNoteController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = await controller.GetById(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async void Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IMaterialDistributionNote>();
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

            var mockFacade = new Mock<IMaterialDistributionNote>();
            mockFacade.Setup(x => x.DeleteModel(It.IsAny<int>()))
                .ReturnsAsync(1);

            var controller = new MaterialDistributionNoteController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Delete(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
