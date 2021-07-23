using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAccessories;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAccessories;
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

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseExpenditureAccessories";

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

        private GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil _dataUtil(GarmentLeftoverWarehouseExpenditureAccessoriesService service)
        {
            return new GarmentLeftoverWarehouseExpenditureAccessoriesDataUtil(service);
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
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            await _dataUtil(service).GetTestData();

            var result = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task ReadById_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = await service.ReadByIdAsync(data.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Create_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Create_Error()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpFailTestService());

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();

            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(data));
        }

        [Fact]
        public async Task Update_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockOut(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataUtil = _dataUtil(service);
            var oldData = dataUtil.GetNewData();
            oldData.Items.Add(new GarmentLeftoverWarehouseExpenditureAccessoriesItem
            {
                StockId = 2,
                UnitId = 2,
                UnitCode = "Unit",
                UnitName = "Unit",
                PONo = "PONo2",
                Quantity = 10,
                UomId = 2,
                UomUnit = "Uom"
            });

            await service.CreateAsync(oldData);

            var newData = dataUtil.CopyData(oldData);
            newData.ExpenditureDate = newData.ExpenditureDate.AddDays(-1);
            newData.Remark = "New" + newData.Remark;
            newData.LocalSalesNoteNo = "New" + newData.LocalSalesNoteNo;
            newData.IsUsed = true;
            var firsItem = newData.Items.First();
            firsItem.Quantity++;
            var lastItem = newData.Items.Last();
            lastItem.Id = 0;

            var result = await service.UpdateAsync(newData.Id, newData);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Error()
        {
            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
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

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Delete_Error()
        {
            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public async Task CheckStockQuantity_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            var stockQuantity = 10;
            stockServiceMock.Setup(s => s.ReadById(It.IsAny<int>()))
                .Returns(new GarmentLeftoverWarehouseStock
                {
                    Quantity = stockQuantity
                });

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            foreach (var item in data.Items)
            {
                var result = service.CheckStockQuantity(data.Id, item.StockId);
                Assert.Equal(stockQuantity, result - item.Quantity);
            }
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var data = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel
            {
                ExpenditureDate = DateTimeOffset.Now,
                ExpenditureDestination = "SAMPLE",
                UnitExpenditure = new UnitViewModel
                {
                    Id = "1",
                    Code = "Unit",
                    Name = "Unit"
                },
                Buyer = new BuyerViewModel
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name"
                },
                EtcRemark = "Remark",
                Remark = "Remark",
                LocalSalesNoteNo = "LocalSalesNoteNo",
                Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>
                {
                    new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel
                    {
                        StockId = 1,
                        Unit = new UnitViewModel{
                            Id = "1",
                            Code = "Unit",
                            Name = "Unit"
                        },
                        PONo = "PONo",
                        Quantity = 1,
                        Uom = new UomViewModel
                        {
                            Id = "1",
                            Unit = "Uom"
                        },
                        Product= new ProductViewModel
                        {
                            Id="1",
                            Code="code",
                            Name="name"
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

            GarmentLeftoverWarehouseExpenditureAccessoriesService service = new GarmentLeftoverWarehouseExpenditureAccessoriesService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateViewModel()
        {
            GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
            {
                ExpenditureDate = null,
                ExpenditureDestination = "",
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);

            GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel2 = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
            {
                ExpenditureDate = DateTimeOffset.MinValue,
                ExpenditureDestination = "JUAL LOKAL",
                Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel()
                        {
                            PONo=null,
                            Product=null
                        }
                    }
            };
            var result2 = viewModel2.Validate(null);
            Assert.True(result2.Count() > 0);

            GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel3 = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
            {
                ExpenditureDate = DateTimeOffset.Now.AddDays(2),
                ExpenditureDestination = "UNIT",
                Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel()
                        {
                        }
                    }
            };
            var result3 = viewModel3.Validate(null);
            Assert.True(result3.Count() > 0);

            GarmentLeftoverWarehouseExpenditureAccessoriesViewModel viewModel4 = new GarmentLeftoverWarehouseExpenditureAccessoriesViewModel()
            {
                ExpenditureDate = DateTimeOffset.Now,
                ExpenditureDestination = "LAIN-LAIN",
                Items = new List<GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel()
                        {
                        }
                    }
            };
            var result4 = viewModel4.Validate(null);
            Assert.True(result4.Count() > 0);

        }
    }
}
