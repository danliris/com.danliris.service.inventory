using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Inventory.Test.Services.InventoryWeaving.InventoryWeavingUpload
{
    public class InventoryWeavingAdjValidationTest
    {
        private InventoryWeavingDocumentOutViewModel ViewModel
        {
            get {

                return new InventoryWeavingDocumentOutViewModel
                {
                    date = DateTimeOffset.MinValue,
                    bonNo = null,
                    bonType = null,
                    storageCode = null,
                    storageId = 0,
                    storageName = null,

                    type = null,
                    remark = null,
                    items = new List<InventoryWeavingItemDetailViewModel>
                    {
                        new InventoryWeavingItemDetailViewModel()
                        {
                            ProductOrderNo = null,
                            ReferenceNo = null,
                            Construction = null,
                            Year = null,
                            ListItems = new List<ItemListDetailViewModel>
                            {
                               new ItemListDetailViewModel()
                               {
                                    Grade = null,
                                    Piece = null,
                                    MaterialName = null,
                                    WovenType = null,
                                    Yarn1 = null,
                                    Yarn2 = null,
                                    YarnType1 = null,
                                    YarnType2 = null,
                                    YarnOrigin1 = null,
                                    YarnOrigin2 = null,
                                    Width = null,
                                    UomUnit = null,
                                    Qty = 0,
                                    QtyPiece = 0,
                                    Quantity = 0,
                                    QuantityPiece =0,
                                    ProductRemark = "",
                                    Barcode = "",
                                    ProductionOrderDate = DateTime.MinValue,
                                    IsSave = false,
                                }
                            }

                        }
                    }
                };
            }
        }

        [Fact]
        public void Validate_DefaultValue()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;
            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_Detail_Qty_Null()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;

            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "PACKING";

            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_Detail_MoreThan()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;
            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "PACKING";
            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.Qty = 2;
                    detail.QtyPiece = 2;
                    detail.Quantity = 1;
                    detail.QuantityPiece = 1;
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJIN_DefaultValue()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;

            viewModel.bonType = "ADJ MASUK";

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJIN_Detail_AllQty_Null()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;

            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "ADJ MASUK";
            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJIN_Detail_Min()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;
            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "ADJ MASUK";
            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.Qty = -1;
                    detail.QtyPiece = -1;
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJOUT_DefaultValue()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;

            viewModel.bonType = "ADJ KELUAR";

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJOUT_Detail_AllQty_Null()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;

            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "ADJ KELUAR";
            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJOUT_Detail_Min()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;
            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "ADJ KELUAR";
            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.Qty = -1;
                    detail.QtyPiece = -1;
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }

        [Fact]
        public void Validate_ADJOUT_Detail_MoreThan()
        {
            InventoryWeavingDocumentOutViewModel viewModel = ViewModel;
            viewModel.date = DateTimeOffset.Now;
            viewModel.bonType = "ADJ KELUAR";
            foreach (var item in viewModel.items)
            {
                foreach (var detail in item.ListItems)
                {
                    detail.Qty = 2;
                    detail.QtyPiece = 2;
                    detail.Quantity = 1;
                    detail.QuantityPiece = 1;
                    detail.IsSave = true;
                }
            }

            var result = viewModel.Validate(null);
            Assert.NotEmpty(result.ToList());
        }
    }
}
