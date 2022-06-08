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
    public class InventoryWeavingDocumentDataUtils
    {
        private readonly InventoryWeavingDocumentUploadService Service;
        public InventoryWeavingDocumentDataUtils(InventoryWeavingDocumentUploadService service)
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
                    Barcode = "barcode",
                    ProductionOrderDate = DateTime.Now,
                    InventoryWeavingDocumentId = 1,
                    
                } }
            };
        }

        public InventoryWeavingItemViewModel GetItemVM()
        {

            return new InventoryWeavingItemViewModel
            {
                Date = DateTimeOffset.Now,
                BonNo = "test01",
                ReferenceNo = "referencce",
                Construction = "CD",
                Grade = "A",
                Piece = "1",
                Quantity = 1,
                QuantityPiece = 1,
                Barcode = "barcode",
                ProductionOrderDate = DateTime.Now,
            };
        }

        public InventoryWeavingDocumentViewModel GetNewData1()
        {
            return new InventoryWeavingDocumentViewModel
            {

                date = DateTimeOffset.Now,
                bonNo = "test01",
                bonType = "PRODUKSI",
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",

                type = "OUT",
                remark = "Remark",
                items = new List<InventoryWeavingDocumentItemViewModel> { new InventoryWeavingDocumentItemViewModel()
                    {
                        productOrderNo = "product",
                        referenceNo = "referencce",
                        construction = "CD",
                        grade = "A",
                        piece = "1",
                        materialName = "CD",
                        wovenType = "",
                        yarn1 = "yarn1",
                        yarn2 = "yarn2",
                        yarnType1 = "yt1",
                        yarnType2 = "yt2",
                        yarnOrigin1 = "yo1",
                        yarnOrigin2 = "yo2",
                        width = "1",
                        uomUnit = "MTR",
                        uomId = 1,
                        quantity = 1,
                        quantityPiece =1,
                        productRemark = "",
                        barcode = "barcode",
                        productionOrderDate = DateTime.Now,
                        InventoryWeavingDocumentId = 1,
                } }
            };
        }
        public InventoryWeavingDocumentCsvViewModel GetNewDataCSV()
        {
            return new InventoryWeavingDocumentCsvViewModel
            {

             
                    
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
                    Qty = 1,
                    QtyPiece = 1,
                    Barcode = "barcode",
                    ProductionOrderDate = DateTime.Now,
                    ProductionOrderNo = "a",
             
            };
        }

        public InventoryWeavingDocumentDetailViewModel UpdateData()
        {
            return new InventoryWeavingDocumentDetailViewModel
            {
                Date = DateTimeOffset.Now,
                BonNo = "test01",
                BonType = "PRODUKSI",
                StorageCode = "a",
                StorageId = 2,
                StorageName = "a",
                Type = "OUT",
                //Remark = "Remark",
                Detail = new List<InventoryWeavingItemDetailViewModel>
                {
                    new InventoryWeavingItemDetailViewModel()
                    {
                        ProductOrderNo = "a",
                        ReferenceNo = "reference",
                        Construction = "CD",
                        Year = "2020",
                        ListItems = new List<ItemListDetailViewModel>
                        {
                            new ItemListDetailViewModel()
                            {
                                Grade = "A",
                                Piece = "1",
                                MaterialName = "CD",
                                WovenType = "SLUB",
                                Yarn1 = "yarn1",
                                Yarn2 = "yarn2",
                                YarnOrigin = "yarnOrigin",
                                YarnOrigin1 = "yarnOrigin1",
                                YarnOrigin2 = "yarnOrigin2",
                                YarnType1 = "yarnType1",
                                YarnType2 = "yarnType2",
                                Width = "1",
                                UomUnit = "MTR",
                                Qty = 1,
                                QtyPiece = 1,
                                ProductRemark = "remark",
                                IsSave = true,
                                Barcode = "barcode",
                                ProductionOrderDate = DateTime.Now,
                            }
                        }
                    }
                }
            };
        }

        public InventoryWeavingDocument GetNewDataAdj()
        {
            return new InventoryWeavingDocument
            {

                Date = DateTimeOffset.Now,
                BonNo = "test01",
                BonType = "PRODUKSI",
                StorageCode = "test01",
                StorageId = 2,
                StorageName = "Test",

                Type = "ADJ_IN",
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

        public InventoryWeavingDocumentViewModel GetNewData1()
        {
            return new InventoryWeavingDocumentViewModel
            {

                date = DateTimeOffset.Now,
                bonNo = "test01",
                bonType = "weaving",
                storageCode = "test01",
                storageId = 2,
                storageName = "Test",

                type = "IN",
                remark = "Remark",
                items = new List<InventoryWeavingDocumentItemViewModel> { new InventoryWeavingDocumentItemViewModel()
                    {
                        productOrderNo = "product",
                        referenceNo = "referencce",
                        construction = "CD",
                        grade = "A",
                        piece = "1",
                        materialName = "CD",
                        wovenType = "",
                        yarn1 = "yarn1",
                        yarn2 = "yarn2",
                        yarnType1 = "yt1",
                        yarnType2 = "yt2",
                        yarnOrigin1 = "yo1",
                        yarnOrigin2 = "yo2",
                        width = "1",
                        uomUnit = "MTR",
                        uomId = 1,
                        quantity = 1,
                        quantityPiece =1,
                        productRemark = "",
                        InventoryWeavingDocumentId = 1
                } }
            };
        }
        public InventoryWeavingDocumentCsvViewModel GetNewDataCSV()
        {
            return new InventoryWeavingDocumentCsvViewModel
            {

             
                    
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
                    Qty = 1,
                    QtyPiece = 1,
                    ProductionOrderNo = "a"
                    
             
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
