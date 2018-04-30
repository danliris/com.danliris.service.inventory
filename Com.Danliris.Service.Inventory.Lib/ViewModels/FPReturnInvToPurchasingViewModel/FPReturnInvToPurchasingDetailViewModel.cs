using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel
{
    public class FPReturnInvToPurchasingDetailViewModel : BaseViewModel<FPReturnInvToPurchasingDetail>
    {
        public int FPReturnInvToPurchasingId { get; set; }
        public int FPRegradingResultDocsId { get; set; }
        public string FPRegradingResultDocsCode { get; set; }
        public ProductViewModel Product { get; set; }
        public double Quantity { get; set; }
        public double NecessaryLength { get; set; }
        public double Length { get; set; }
        public string Description { get; set; }

        public FPReturnInvToPurchasingDetailViewModel() { }

        public FPReturnInvToPurchasingDetailViewModel(FPReturnInvToPurchasingDetail model)
        {
            PropertyCopier<FPReturnInvToPurchasingDetail, FPReturnInvToPurchasingDetailViewModel>.Copy(model, this);

            #region Product

            this.Product = new ProductViewModel();
            this.Product._id = model.ProductId;
            this.Product.code = model.ProductCode;
            this.Product.name = model.ProductName;

            #endregion Product
        }

        public override FPReturnInvToPurchasingDetail ToModel()
        {
            FPReturnInvToPurchasingDetail model = new FPReturnInvToPurchasingDetail();
            PropertyCopier<FPReturnInvToPurchasingDetailViewModel, FPReturnInvToPurchasingDetail>.Copy(this, model);

            #region Product

            model.ProductId = this.Product._id;
            model.ProductCode = this.Product.code;
            model.ProductName = this.Product.name;

            #endregion Product

            return model;
        }
    }
}
