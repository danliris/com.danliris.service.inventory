using Com.Danliris.Service.Inventory.Lib.Models;
using Com.Danliris.Service.Inventory.Lib.Models.FPReturnInvToPurchasingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.FPReturnInvToPurchasingDataUtil
{
    public class FPReturnInvToPurchasingDetailDataUtil
    {
        public List<FPReturnInvToPurchasingDetail> GetNewData(FpRegradingResultDocs fpRegradingResultDocs)
        {
            List<FPReturnInvToPurchasingDetail> list = new List<FPReturnInvToPurchasingDetail>();

            foreach(FpRegradingResultDocsDetails detail in fpRegradingResultDocs.Details)
            {
                list.Add(new FPReturnInvToPurchasingDetail
                {
                    FPRegradingResultDocsId = fpRegradingResultDocs.Id,
                    FPRegradingResultDocsCode = fpRegradingResultDocs.Code,
                    ProductId = detail.ProductId,
                    ProductCode = detail.ProductCode,
                    ProductName = detail.ProductName,
                    Quantity = 1,
                    NecessaryLength = 1,
                    Length = detail.Length,
                    Description = ""
                });
            }

            return list;
        }
    }
}
