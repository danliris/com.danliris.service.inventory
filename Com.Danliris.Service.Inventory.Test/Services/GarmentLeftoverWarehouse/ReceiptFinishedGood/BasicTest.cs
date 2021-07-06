using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodViewModel;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFinishedGoodServiceTests
{
    public class BasicTest
    {
        const string ENTITY = "GarmentLeftoverWarehouseReceiptFinishedGood";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private InventoryDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<InventoryDbContext> optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            InventoryDbContext dbContext = new InventoryDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private GarmentLeftoverWarehouseReceiptFinishedGoodDataUtil _dataUtil(GarmentLeftoverWarehouseReceiptFinishedGoodService service)
        {
            return new GarmentLeftoverWarehouseReceiptFinishedGoodDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        [Fact]
        public async Task Read_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            await _dataUtil(service).GetTestData();

            var result = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task ReadById_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Create_Error()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpFailTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();

            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(data));
        }

        [Fact]
        public async Task Update_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataUtil = _dataUtil(service);
            var oldData = await dataUtil.GetTestData();

            var newData = dataUtil.CopyData(oldData);
            newData.ReceiptDate = DateTimeOffset.Now;
            newData.Description = "New Remark";

            var result = await service.UpdateAsync(newData.Id, newData);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Error()
        {
            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(0, null));
        }

        [Fact]
        public async Task Delete_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Delete_Error()
        {
            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var data = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel
            {
                UnitFrom = new UnitViewModel
                {
                    Id = "1",
                    Code = "Unit",
                    Name = "Unit"
                },
                ExpenditureDate = DateTimeOffset.Now,
                ReceiptDate = DateTimeOffset.Now,
                Description = "Remark",
                Carton=1,
                ContractNo="noContract",
                ExpenditureDesc="desc",
                Invoice="inv",
                Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>
                    {
                        new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel
                        {
                            ExpenditureGoodItemId = Guid.NewGuid(),
                            Size = new SizeViewModel{
                                Id = 1,
                                Name = "Size",
                                Code="Code"
                            },
                            Remark = "Remark",
                            Quantity = 1,
                            Uom = new UomViewModel
                            {
                                Id = "1",
                                Unit = "Uom"
                            },
                            LeftoverComodity= new LeftoverComodityViewModel
                            {
                                Id=1,
                                Name="como",
                                Code="como"
                            },
                            ExpenditureGoodId = Guid.NewGuid(),
                            ExpenditureGoodNo = "no",
                            Buyer = new BuyerViewModel
                            {
                                Id = 1,
                                Code = "Buyer",
                                Name = "Buyer"
                            },
                            Comodity= new ComodityViewModel
                            {
                                Id = 1,
                                Code = "Comodity",
                                Name = "Comodity"
                            },
                            RONo="roNo",
                            Article="art",
                        }
                    }
            };

            var result = service.MapToModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task MapToViewModel()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseReceiptFinishedGoodService service = new GarmentLeftoverWarehouseReceiptFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateViewModel()
        {
            GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel()
            {
                UnitFrom = null,
                ReceiptDate = DateTimeOffset.MinValue,
                Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel()
                    }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);

            GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel1 = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel()
            {
                UnitFrom = null,
                ReceiptDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel()
                    }
            };
            var result1 = viewModel1.Validate(null);
            Assert.True(result1.Count() > 0);


            GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel2 = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel()
            {
                UnitFrom = null,
                ReceiptDate = DateTimeOffset.Now.AddDays(4)
            };
            var result2 = viewModel2.Validate(null);
            Assert.True(result2.Count() > 0);

            GarmentLeftoverWarehouseReceiptFinishedGoodViewModel viewModel3 = new GarmentLeftoverWarehouseReceiptFinishedGoodViewModel()
            {
                UnitFrom = null,
                ReceiptDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseReceiptFinishedGoodItemViewModel()
                        {
                            ExpenditureGoodNo=null,
                            LeftoverComodity= new LeftoverComodityViewModel
                            {
                                Name="name",
                                Id=1
                            }
                        }
                    }
            };
            var result3 = viewModel3.Validate(null);
            Assert.True(result3.Count() > 0);
        }

        [Fact]
        private async Task TestPatchError()
        {
            HttpService httpService = new HttpService(new IdentityService());
            await Assert.ThrowsAnyAsync<Exception>(() => httpService.PutAsync(null, null));
        }

        
    }
}
