using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalServices;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Mutation;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Stock;
using Com.Danliris.Service.Inventory.Lib.ViewModels;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Mutation;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.ExpenditureAval;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class BasicTest
    {
        const string ENTITY = "GarmentLeftoverWarehouseExpenditureAval";

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

        private GarmentLeftoverWarehouseExpenditureAvalDataUtil _dataUtil(GarmentLeftoverWarehouseExpenditureAvalService service, string testName)
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

            GarmentLeftoverWarehouseReceiptAvalService receiptService = new GarmentLeftoverWarehouseReceiptAvalService(_dbContext(testName), serviceProvider.Object);
            
            GarmentLeftoverWarehouseReceiptAvalDataUtil ReceiptAvalDataUtil = new GarmentLeftoverWarehouseReceiptAvalDataUtil(receiptService);
            return new GarmentLeftoverWarehouseExpenditureAvalDataUtil(service,ReceiptAvalDataUtil);
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

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            await _dataUtil(service,GetCurrentMethod()).GetTestDataFabric();

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


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service, GetCurrentMethod()).GetTestDataFabric();

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


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service, GetCurrentMethod()).GetNewDataFabric();

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


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data =  _dataUtil(service, GetCurrentMethod()).GetNewDataAcc();

            var result = await service.CreateAsync(data);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Create_Error()
        {
            var serviceProvider = GetServiceProvider();

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service, GetCurrentMethod()).GetNewDataFabric();

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


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataUtil =  _dataUtil(service, GetCurrentMethod());
            var oldData = await dataUtil.GetNewDataFabric();
            var newItem = await dataUtil.GetNewDataItemFabric();
            
            await service.CreateAsync(oldData);

            var newData = dataUtil.CopyData(oldData);
            newData.ExpenditureDate = newData.ExpenditureDate.AddDays(-1);
            newData.Description = "New" + newData.Description;
            newData.LocalSalesNoteNo = "New" + newData.LocalSalesNoteNo;
            newData.Items.Add(newItem);
            var lastItem = newData.Items.Last();
            lastItem.Id = 0;

            var result = await service.UpdateAsync(newData.Id, newData);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Update_Success_Acc()
        {
            var serviceProvider = GetServiceProvider();

            var stockServiceMock = new Mock<IGarmentLeftoverWarehouseStockService>();
            stockServiceMock.Setup(s => s.StockIn(It.IsAny<GarmentLeftoverWarehouseStock>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            serviceProvider
                .Setup(x => x.GetService(typeof(IGarmentLeftoverWarehouseStockService)))
                .Returns(stockServiceMock.Object);


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var dataUtil = _dataUtil(service, GetCurrentMethod());
            var oldData = dataUtil.GetNewDataAcc();
            oldData.Items.Add(new GarmentLeftoverWarehouseExpenditureAvalItem
            {
                StockId = 1,
                UnitId = 1,
                UnitCode = "Unit",
                UnitName = "Unit",
                Quantity = 1,
                UomId = 1,
                UomUnit = "Uom",
                ProductCode = "code",
                ProductId = 1,
                ProductName = "product"
            });

            await service.CreateAsync(oldData);

            var newData = dataUtil.CopyData(oldData);
            newData.Description = "New" + newData.Description;
            newData.LocalSalesNoteNo = "New" + newData.LocalSalesNoteNo;
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
            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
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


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service, GetCurrentMethod()).GetTestDataFabric();

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


            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = _dataUtil(service, GetCurrentMethod()).GetTestDataAcc();

            var result = await service.DeleteAsync(data.Id);

            Assert.NotEqual(0, result);
        }

        [Fact]
        public async Task Delete_Error()
        {
            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }

        [Fact]
        public void MapToModel()
        {
            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), GetServiceProvider().Object);

            var data = new GarmentLeftoverWarehouseExpenditureAvalViewModel
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
                LocalSalesNoteNo = "LocalSalesNoteNo",
                OtherDescription = "sadd",
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>
                    {
                        new GarmentLeftoverWarehouseExpenditureAvalItemViewModel
                        {
                            AvalReceiptNo = "roNo",
                            Unit = new UnitViewModel
                            {
                                Id = "1",
                                Code = "Unit",
                                Name = "Unit"
                            },
                            Quantity=1,
                            AvalReceiptId=1,
                            Product= new ProductViewModel
                            {
                                Id = "1",
                                Code = "Product",
                                Name = "Product"
                            },
                            Uom= new UomViewModel
                            {
                                Id="1",
                                Unit="Uom"
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

            GarmentLeftoverWarehouseExpenditureAvalService service = new GarmentLeftoverWarehouseExpenditureAvalService(_dbContext(GetCurrentMethod()), serviceProvider.Object);

            var data = await _dataUtil(service, GetCurrentMethod()).GetTestDataFabric();

            var result = service.MapToViewModel(data);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateViewModel_Fabric()
        {
            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                Buyer = null,
                ExpenditureTo = "JUAL LOKAL",
                AvalType="AVAL FABRIC",
                ExpenditureDate = DateTimeOffset.MinValue,
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAvalItemViewModel()
                    }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);

            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel1 = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                Buyer = null,
                ExpenditureTo = "LAIN-LAIN",
                OtherDescription = "",
                AvalType = "AVAL FABRIC",
                ExpenditureDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAvalItemViewModel()
                        {
                            Quantity=6
                        }
                    }
            };
            var result1 = viewModel1.Validate(null);
            Assert.True(result1.Count() > 0);


            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel2 = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                Buyer = null,
                ExpenditureTo = "LAIN-LAIN",
                OtherDescription = "",
                AvalType = "AVAL FABRIC",
                ExpenditureDate = DateTimeOffset.Now.AddDays(4)
            };
            var result2 = viewModel2.Validate(null);
            Assert.True(result2.Count() > 0);
        }

        [Fact]
        public void ValidateViewModel_Acc()
        {
            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                Buyer = null,
                ExpenditureTo = "JUAL LOKAL",
                AvalType = "AVAL ACCESSORIES",
                ExpenditureDate = DateTimeOffset.MinValue,
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAvalItemViewModel()
                    }
            };
            var result = viewModel.Validate(null);
            Assert.True(result.Count() > 0);

            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel1 = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                Buyer = null,
                ExpenditureTo = "LAIN-LAIN",
                OtherDescription = "",
                AvalType = "AVAL ACCESSORIES",
                ExpenditureDate = DateTimeOffset.Now.AddDays(4),
                Items = new List<GarmentLeftoverWarehouseExpenditureAvalItemViewModel>()
                    {
                        new GarmentLeftoverWarehouseExpenditureAvalItemViewModel()
                        {
                            Quantity=6,
                            StockQuantity=1
                        }
                    }
            };
            var result1 = viewModel1.Validate(null);
            Assert.True(result1.Count() > 0);


            GarmentLeftoverWarehouseExpenditureAvalViewModel viewModel2 = new GarmentLeftoverWarehouseExpenditureAvalViewModel()
            {
                Buyer = null,
                ExpenditureTo = "LAIN-LAIN",
                OtherDescription = "",
                AvalType = "AVAL ACCESSORIES",
                ExpenditureDate = DateTimeOffset.Now.AddDays(4)
            };
            var result2 = viewModel2.Validate(null);
            Assert.True(result2.Count() > 0);
        }

        [Fact]
        public void MutationScrap()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("scrap-transactions/mutation"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<GarmentLeftoverWarehouseMutationReportViewModel>())) });


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseMutationReportService service = new GarmentLeftoverWarehouseMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);

            var result = service.GetMutation(null, null, 1, 25);

            Assert.True(result.Item1.Count() > 0);


        }

        [Fact]
        public void Excel_MutationScrap()
        {
            var serviceProvider21 = new Mock<IServiceProvider>();

            var httpClientService = new Mock<IHttpService>();

            httpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("scrap-transactions/mutation"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonConvert.SerializeObject(new List<GarmentLeftoverWarehouseMutationReportViewModel>())) });


            serviceProvider21
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider21
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(httpClientService.Object);

            GarmentLeftoverWarehouseMutationReportService service = new GarmentLeftoverWarehouseMutationReportService(_dbContext(GetCurrentMethod()), serviceProvider21.Object);

            var result = service.GenerateExcelMutation(DateTime.Now, DateTime.Now);

            Assert.IsType<MemoryStream>(result);


        }
    }
}
