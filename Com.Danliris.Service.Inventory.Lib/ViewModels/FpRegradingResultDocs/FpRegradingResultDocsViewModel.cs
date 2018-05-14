using Com.Danliris.Service.Inventory.Lib.Helpers;
using Com.Danliris.Service.Inventory.Lib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels
{
    public class FpRegradingResultDocsViewModel : BasicViewModel, IValidatableObject
    {
        public string Code { get; set; }
        public DateTimeOffset? Date { get; set; }
        public noBon Bon { get; set; }
        public supplier Supplier { get; set; }
        public product Product { get; set; }
        public string Operator { get; set; }
        public machine Machine { get; set; }
        public string Remark { get; set; }
        public string Shift { get; set; }
        public double TotalLength { get; set; }
        public string OriginalGrade { get; set; }
        public bool IsReturn { get; set; }
        public bool IsReturnedToPurchasing { get; set; }
        public List<FpRegradingResultDetailsDocsViewModel> Details { get; set; }

        public class supplier
        {
            public string _id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
        }
        public class machine
        {
            public string _id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
        }
        public class product
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
        }

        public class noBon
        {
            public string _id { get; set; }
            public string no { get; set; }
            public string unitName { get; set; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {


            if (this.Bon == null || (this.Bon._id) == "")
                yield return new ValidationResult("Bon harus di isi", new List<string> { "Bon" });


            if (this.Machine == null || (this.Machine._id) == "")
                yield return new ValidationResult("Mesin harus di isi", new List<string> { "Machine" });

            if (this.Operator == null || string.IsNullOrWhiteSpace(this.Operator))
                yield return new ValidationResult("Operator harus di isi", new List<string> { "Operator" });

            if (this.Date == null)
                yield return new ValidationResult("Tanggal harus di isi", new List<string> { "Date" });

            int Count = 0;
            string detailsError = "[";
            if (this.Details.Count.Equals(0) || this.Details == null)
            {
                yield return new ValidationResult("Details is required", new List<string> { "Details" });
            }
            else
            {
                //List<string> temp = new List<string>();
                foreach (FpRegradingResultDetailsDocsViewModel data in this.Details)
                {
                    detailsError += "{";
                    //if (data.Product == null || string.IsNullOrWhiteSpace(data.Product.Name))
                    //{
                    //    Count++;
                    //    detailsError += "Product: 'Barang harus di isi',";
                    //}

                    //if (data.Product != null)
                    //{
                    //if (!(data.LengthBeforeReGrade.Equals(0)))
                    //{
                    //    if ((data.Length > data.LengthBeforeReGrade) || data.Length.Equals(0))
                    //    {
                    //        Count++;
                    //        detailsError += "Length: 'Panjang harus lebih kecil',";
                    //    }
                    //}


                    //if (data.Product.Name != null || data.Product.Name != "")
                    //{
                    //    if (!(temp.Contains(data.Product.Name)))
                    //    {
                    //        temp.Add(data.Product.Name);
                    //    }
                    //    else
                    //    {
                    //        Count++;
                    //        detailsError += "Product: 'Barang tidak boleh sama',";
                    //    }
                    //}

                    //}
                    if (data.Length.Equals(0))
                    {
                        Count++;
                        detailsError += "Length: 'Panjang harus di isi',";
                    }

                    if (data.Quantity.Equals(0))
                    {
                        Count++;
                        detailsError += "Quantity: 'Jumlah harus di isi',";
                    }
                    //if (data.LengthBeforeReGrade.Equals(0))
                    //{
                    //    Count++;
                    //    detailsError += "LengthBeforeReGrade: 'harus di isi',";
                    //}
                    detailsError += "},";

                }

                detailsError += "]";
            }
            if (Count > 0)
            {
                yield return new ValidationResult(detailsError, new List<string> { "Details" });
            }
        }
    }
}
