using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalDataUtils;
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

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServiceTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseReceiptAval";

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

        private GarmentLeftoverWarehouseReceiptAvalDataUtil _dataUtil(GarmentLeftoverWarehouseReceiptAvalService service)
        {
            return new GarmentLeftoverWarehouseReceiptAvalDataUtil(service);
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_Success_FABRIC()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Create_Success_ACC()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();
            data.AvalType = "AVAL ACCESSORIES";
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataUtil = _dataUtil(service);
            var oldData = await dataUtil.GetTestData();

            var newData = dataUtil.CopyData(oldData);
            newData.ReceiptDate = DateTimeOffset.Now;
            newData.Remark = "New Remark";
            newData.TotalAval = oldData.TotalAval + 3;

            var result = await service.UpdateAsync(newData.Id, newData);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Error()
        {
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(0, null));
        }

        [Fact]
        public async Task Delete_Success_FABRIC()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();
            
            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Delete_Success_ACC()
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestDataACC();

            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Delete_Error()
        {
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var data = new GarmentLeftoverWarehouseReceiptAvalViewModel
            {
                UnitFrom = new UnitViewModel
                {
                    Id = "1",
                    Code = "Unit",
                    Name = "Unit"
                },
                AvalType = "AVAL FABRIC",
                TotalAval = 10,
                ReceiptDate = DateTimeOffset.Now,
                Remark = "Remark",
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel
                    {
                        RONo = "ro",
                        Product = new ProductViewModel{
                            Id = "1",
                            Code = "Product",
                            Name = "Product"
                        },
                        ProductRemark = "Remark",
                        Quantity = 1,
                        Uom = new UomViewModel
                        {
                            Id = "1",
                            Unit = "Uom"
                        }
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

            GarmentLeftoverWarehouseReceiptAvalService service = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateViewModel()
        {
            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel = new GarmentLeftoverWarehouseReceiptAvalViewModel()
            {
                UnitFrom = null,
                TotalAval = 0,
                AvalType="AVAL FABRIC",
                ReceiptDate = DateTimeOffset.MinValue,
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>()
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel()
                }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);

            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel1 = new GarmentLeftoverWarehouseReceiptAvalViewModel()
            {
                UnitFrom = null,
                TotalAval = 0,
                AvalType = "AVAL FABRIC",
                ReceiptDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>()
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel()
                    {
                        RONo="error"
                    }
                }
            };
            var result1 = viewModel1.Validate(null);
            Assert.True(result1.Count() > 0);

            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel2 = new GarmentLeftoverWarehouseReceiptAvalViewModel()
            {
                UnitFrom = null,
                AvalType = "AVAL ACCESSORIES",
                ReceiptDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>()
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel()
                    {
                        Product=null,
                        Quantity=0,
                        Uom=null
                    }
                }
            };
            var result2 = viewModel2.Validate(null);
            Assert.True(result2.Count() > 0);

            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel3 = new GarmentLeftoverWarehouseReceiptAvalViewModel()
            {
                UnitFrom = null,
                AvalType = "AVAL ACCESSORIES",
                ReceiptDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>()
                {
                   
                }
            };
            var result3 = viewModel3.Validate(null);
            Assert.True(result3.Count() > 0);

            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel4 = new GarmentLeftoverWarehouseReceiptAvalViewModel()
            {
                UnitFrom = null,
                AvalType = "AVAL BAHAN PENOLONG",
                ReceiptDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>()
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel()
                    {
                        Product=null,
                        Quantity=0,
                        Uom=null
                    }
                }
            };
            var result4 = viewModel4.Validate(null);
            Assert.True(result4.Count() > 0);

            GarmentLeftoverWarehouseReceiptAvalViewModel viewModel5 = new GarmentLeftoverWarehouseReceiptAvalViewModel()
            {
                UnitFrom = null,
                AvalType = "AVAL KOMPONEN",
                ReceiptDate = null,
                Items = new List<GarmentLeftoverWarehouseReceiptAvalItemViewModel>()
                {
                    new GarmentLeftoverWarehouseReceiptAvalItemViewModel()
                    {
                        Product=null,
                        Quantity=0,
                        Uom=null,
                        AvalComponentNo=null
                    }
                }
            };
            var result5 = viewModel5.Validate(null);
            Assert.True(result5.Count() > 0);
        }

        
    }
}
