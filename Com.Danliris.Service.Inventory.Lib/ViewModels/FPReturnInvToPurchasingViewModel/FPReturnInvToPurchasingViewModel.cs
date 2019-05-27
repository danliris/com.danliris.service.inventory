using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Interfaces;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.FPReturnInvToPurchasingViewModel
{
    public class FPReturnInvToPurchasingViewModel : BaseViewModel<FPReturnInvToPurchasing>, IValidatableObject
    {
        public string No { get; set; }
        public UnitViewModel Unit { get; set; }
        public SupplierViewModel Supplier { get; set; }
        public int AutoIncrementNumber { get; set; }
        public List<FPReturnInvToPurchasingDetailViewModel> FPReturnInvToPurchasingDetails { get; set; }

        public FPReturnInvToPurchasingViewModel() { }

        public FPReturnInvToPurchasingViewModel(FPReturnInvToPurchasing model)
        {
            PropertyCopier<FPReturnInvToPurchasing, FPReturnInvToPurchasingViewModel>.Copy(model, this);

            #region Unit

            this.Unit = new UnitViewModel();
            this.Unit.Name = model.UnitName;

            #endregion Unit

            #region Supplier

            this.Supplier = new SupplierViewModel();
            this.Supplier._id = model.SupplierId;
            this.Supplier.code = model.SupplierCode;
            this.Supplier.name = model.SupplierName;

            #endregion Supplier

            this.FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetailViewModel>();
            foreach (FPReturnInvToPurchasingDetail detail in model.FPReturnInvToPurchasingDetails)
            {
                FPReturnInvToPurchasingDetailViewModel detailVM = new FPReturnInvToPurchasingDetailViewModel(detail);
                this.FPReturnInvToPurchasingDetails.Add(detailVM);
            }
        }

        public override FPReturnInvToPurchasing ToModel()
        {
            FPReturnInvToPurchasing model = new FPReturnInvToPurchasing();
            PropertyCopier<FPReturnInvToPurchasingViewModel, FPReturnInvToPurchasing>.Copy(this, model);

            #region Unit

            model.UnitName = this.Unit.Name;

            #endregion Unit

            #region Supplier

            model.SupplierId = this.Supplier._id;
            model.SupplierCode = this.Supplier.code;
            model.SupplierName = this.Supplier.name;

            #endregion Supplier

            model.FPReturnInvToPurchasingDetails = new List<FPReturnInvToPurchasingDetail>();
            foreach (FPReturnInvToPurchasingDetailViewModel detailVM in this.FPReturnInvToPurchasingDetails)
            {
                FPReturnInvToPurchasingDetail detail = detailVM.ToModel();
                model.FPReturnInvToPurchasingDetails.Add(detail);
            }

            return model;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            int Count = 0;

            if (this.Unit == null || string.IsNullOrWhiteSpace(this.Unit.Name))
                yield return new ValidationResult("Unit is required", new List<string> { "Unit" });

            if (this.Supplier == null || string.IsNullOrWhiteSpace(this.Supplier._id))
                yield return new ValidationResult("Supplier is required", new List<string> { "Supplier" });

            if (FPReturnInvToPurchasingDetails.Count.Equals(0))
            {
                yield return new ValidationResult("Details is required", new List<string> { "FPReturnInvToPurchasingCollection" });
            }
            else
            {
                string DetailError = "[";

                foreach (FPReturnInvToPurchasingDetailViewModel detail in this.FPReturnInvToPurchasingDetails)
                {
                    if (string.IsNullOrWhiteSpace(detail.FPRegradingResultDocsCode))
                    {
                        Count++;
                        DetailError += "{ fpRegradingResultDocs: 'Regrading Result Docs No is required' }, ";
                    }

                    else if (detail.NecessaryLength <= 0)
                    {
                        Count++;
                        DetailError += "{ NecessaryLength: 'Necessary Length must be greater than zero' }, ";
                    }
                }

                if (Count > 0)
                {
                    yield return new ValidationResult(DetailError, new List<string> { "FPReturnInvToPurchasingDetails" });
                }
            }
        }
    }
}
