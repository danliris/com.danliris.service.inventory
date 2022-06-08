using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.InventoryWeavingModel;
using Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving;
using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.InventoryWeavingDataUtils
{
    public class InventoryWeavingDocumentOutDataUtil
    {
        private readonly InventoryWeavingDocumentOutService Service;
        public InventoryWeavingDocumentOutDataUtil(InventoryWeavingDocumentOutService service)
        {
            Service = service;
        }

        public InventoryWeavingDocument GetNewData()
        {
            return new InventoryWeavingDocument
            {

                Date = DateTimeOffset.Now,
                BonNo = "test01",
                BonType = "PRODUKSI",
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",

                Type = "OUT",
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
                    Barcode = "barcode",
                    ProductionOrderDate = DateTime.Now,
                } }
            };
        }

        public InventoryWeavingDocumentOutViewModel GetNewData1()
        {
            return new InventoryWeavingDocumentOutViewModel
            {

                date = DateTimeOffset.Now,
                bonNo = "test01",
                bonType = "PRODUKSI",
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",

                type = "OUT",
                remark = "Remark",
                items = new List<InventoryWeavingItemDetailViewModel> { new InventoryWeavingItemDetailViewModel()
                    {
                        ProductOrderNo = "product",
                        ReferenceNo = "referencce",
                        Construction = "CD",
                        Year = "year",
                        ListItems = new List<ItemListDetailViewModel> { new ItemListDetailViewModel()
                        {
                            Grade = "A",
                            //Piece = "1",
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

                } }
            };
        }

        public InventoryWeavingDocumentOutUploadViewModel GetNewData2()
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

        public InventoryWeavingInOutViewModel GetMaterialList()
        {
            return new InventoryWeavingInOutViewModel
            {
                Nomor = 1,
                Construction = "CD",
                Grade = "a",
                Piece = "1",
                Type = "A",
                MaterialName = "CD",
                WovenType = "a",
                Yarn1 = "yarn1",
                Yarn2 = "yarn2",
                YarnType1 = "yt1",
                YarnType2 = "yt2",
                YarnOrigin1 = "yo1",
                YarnOrigin2 = "yo2",
                Width = "1",
                UomUnit = "MTR2",
                Qty = 1,
                QtyPiece = 1,
                Barcode = "barcode",
                ProductionOrderDate = DateTime.Now,
            };
        }

        public InventoryWeavingDocumentOutViewModel GetCSVDownloadOut()
        {
            return new InventoryWeavingDocumentOutViewModel
            {
                date = DateTimeOffset.Now,
                bonNo = "test01",
                bonType = "PRODUKSI",
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",

                type = "OUT",
                remark = "Remark",
                items = new List<InventoryWeavingItemDetailViewModel> { new InventoryWeavingItemDetailViewModel()
                    {
                        ProductOrderNo = "product",
                        ReferenceNo = "referencce",
                        Construction = "CD",
                        Year = "year",
                        ListItems = new List<ItemListDetailViewModel> { new ItemListDetailViewModel()
                        {
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
                } }
            };
        }

        public InventoryWeavingDocumentDetailViewModel GetModelDetail()
        {
            return new InventoryWeavingDocumentDetailViewModel
            {
                Date = DateTimeOffset.Now,
                BonNo = "test01",
                BonType = "PRODUKSI",
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",

                Type = "OUT",
                Detail = new List<InventoryWeavingItemDetailViewModel>
                {
                    new InventoryWeavingItemDetailViewModel()
                    {
                        ProductOrderNo = "product",
                        ReferenceNo = "referencce",
                        Construction = "CD",
                        Year = "year",
                        ListItems = new List<ItemListDetailViewModel>
                        {
                            new ItemListDetailViewModel()
                            {
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
                            }
                        }
                    }
                }
            };
        }

        public InventoryWeavingOutReportViewModel GetQueryReport()
        {
            return new InventoryWeavingOutReportViewModel
            {
                Number = 1,
                Date = DateTimeOffset.Now,
                BonNo = "test01",
                Construction = "CD",
                Grade = "A",
                Piece = "1",
                Quantity = 1,
                QuantityPiece = 1,
                QuantityTot = 1,
                QuantityPieceTot = 1,
                Remark = "",
                Id = 1,
                Barcode = "barcode",
                ProductionOrderDate = DateTime.Now,
            };
        }

        public async Task<InventoryWeavingDocument> GetTestData()
        {
            InventoryWeavingDocument invWDoc = GetNewData();

            await Service.Create(invWDoc);

            return invWDoc;
        }

        public InventoryWeavingDocument CopyData(InventoryWeavingDocument oldData)
        {
            InventoryWeavingDocument newData = new InventoryWeavingDocument();

            PropertyCopier<InventoryWeavingDocument, InventoryWeavingDocument>.Copy(oldData, newData);

            newData.Items = new List<InventoryWeavingDocumentItem>();
            foreach (var oldItem in oldData.Items)
            {
                InventoryWeavingDocumentItem newItem = new InventoryWeavingDocumentItem();

                PropertyCopier<InventoryWeavingDocumentItem, InventoryWeavingDocumentItem>.Copy(oldItem, newItem);

                newData.Items.Add(newItem);
            }

            return newData;
        }
    }
}
