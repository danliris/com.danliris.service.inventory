using Com.Danliris.Service.Inventory.Lib;
using Com.Danliris.Service.Inventory.Lib.Services;
using Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricServices;
using Com.Danliris.Service.Inventory.Test.DataUtils.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricDataUtils;
using Com.Danliris.Service.Inventory.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Facades.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptFabricFacadeTests
{
    public class BasicTest
    {
        private const string ENTITY = "GarmentLeftoverWarehouseReceiptFabric";

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

        private GarmentLeftoverWarehouseReceiptFabricDataUtil _dataUtil(GarmentLeftoverWarehouseReceiptFabricService service)
        {
            return new GarmentLeftoverWarehouseReceiptFabricDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpService)))
                .Returns(new HttpTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            return serviceProvider;
        }

        [Fact]
        public void Read()
        {
            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService();
            Assert.ThrowsAny<Exception>(() => service.Read(1, 1, null, null, null, null));
        }

        [Fact]
        public async Task ReadById()
        {
            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService();
            await Assert.ThrowsAnyAsync<Exception>(() => service.ReadByIdAsync(0));
        }

        [Fact]
        public async Task Create()
        {
            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService();
            await Assert.ThrowsAnyAsync<Exception>(() => service.CreateAsync(_dataUtil(service).GetNewData()));
        }

        [Fact]
        public async Task Update()
        {
            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService();
            await Assert.ThrowsAnyAsync<Exception>(() => service.UpdateAsync(0, _dataUtil(service).GetNewData()));
        }

        [Fact]
        public async Task Delete()
        {
            GarmentLeftoverWarehouseReceiptFabricService service = new GarmentLeftoverWarehouseReceiptFabricService();
            await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteAsync(0));
        }
    }
}
