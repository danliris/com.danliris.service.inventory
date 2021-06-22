using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureFinishedGood;
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

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class BasicTest
    {
        const string ENTITY = "GarmentLeftoverWarehouseExpenditureFinishedGood";

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

        private GarmentLeftoverWarehouseExpenditureFinishedGoodDataUtil _dataUtil(GarmentLeftoverWarehouseExpenditureFinishedGoodService service)
        {
            return new GarmentLeftoverWarehouseExpenditureFinishedGoodDataUtil(service);
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


            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            await _dataUtil(service).GetTestData();

            var result = service.Read(1, 25, "{}", null, null, "{}");

            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Read_Selected_Success()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            await _dataUtil(service).GetTestData();

            List<string> select = new List<string>
            {
                "FinishedGoodExpenditureNo"
            };

            var result = service.Read(1, 25, "{}", select, null, "{}");

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


            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

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


            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service).GetNewData();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Create_Error()
        {
            var serviceProvider = GetServiceProvider();

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

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


            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataUtil = _dataUtil(service);
            var oldData = dataUtil.GetNewData();

            oldData.Items.Add(new GarmentLeftoverWarehouseExpenditureFinishedGoodItem
            {
                StockId = 2,
                UnitId = 2,
                UnitCode = "Unit",
                UnitName = "Unit",
                ExpenditureQuantity = 10,
            });

            await service.CreateAsync(oldData);

            var newData = dataUtil.CopyData(oldData);
            newData.ExpenditureDate = newData.ExpenditureDate.AddDays(-1);
            newData.Description = "New" + newData.Description;
            newData.LocalSalesNoteNo = "New" + newData.LocalSalesNoteNo;
            var firsItem = newData.Items.First();
            firsItem.ExpenditureQuantity++;
            var lastItem = newData.Items.Last();
            lastItem.Id = 0;

            var result = await service.UpdateAsync(newData.Id, newData);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Error()
        {
            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
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


            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Delete_Error()
        {
            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var data = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel
            {
                Buyer = new BuyerViewModel
                {
                    Id = 1,
                    Code = "Buyer",
                    Name = "Buyer"
                },
                ExpenditureDate = DateTimeOffset.Now,
                ExpenditureTo = "JUAL LOKAL",
                Description = "Remark",
                LocalSalesNoteNo ="LocalSalesNoteNo",
                OtherDescription = "sadd",
                Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>
                    {
                        new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel
                        {
                            RONo = "roNo",
                            Unit = new UnitViewModel
                            {
                                Id = "1",
                                Code = "Unit",
                                Name = "Unit"
                            },
                            ExpenditureQuantity=1,
                            StockQuantity=2,
                            LeftoverComodity= new LeftoverComodityViewModel
                            {
                                Id = 1,
                                Code = "code",
                                Name = "name"
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

            GarmentLeftoverWarehouseExpenditureFinishedGoodService service = new GarmentLeftoverWarehouseExpenditureFinishedGoodService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service).GetTestData();

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateViewModel()
        {
            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel()
            {
                Buyer = null,
                ExpenditureTo = "JUAL LOKAL",
                ExpenditureDate = DateTimeOffset.MinValue,
                Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel()
                    }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);

            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel1 = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel()
            {
                Buyer = null,
                ExpenditureTo = "LAIN-LAIN",
                OtherDescription="",
                ExpenditureDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel()
                        {
                            ExpenditureQuantity=6,
                            StockQuantity=3
                        }
                    }
            };
            var result1 = viewModel1.Validate(null);
            Assert.True(result1.Count() > 0);


            GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel viewModel2 = new GarmentLeftoverWarehouseExpenditureFinishedGoodViewModel()
            {
                Buyer = null,
                ExpenditureTo = "LAIN-LAIN",
                OtherDescription = "",
                ExpenditureDate = DateTimeOffset.Now.AddDays(4)
            };
            var result2 = viewModel2.Validate(null);
            Assert.True(result2.Count() > 0);
        }
    }
}
