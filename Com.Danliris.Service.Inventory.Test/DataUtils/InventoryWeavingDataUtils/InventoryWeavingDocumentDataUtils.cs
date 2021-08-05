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
             
                Type = "IN",
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
                } }
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
