using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Danliris.Service.Inventory.Lib.Services.MaterialsRequestNoteServices
{
    public class MaterialsRequestNote_ItemService : BasicService<InventoryDbContext, MaterialsRequestNote_Item>
    {
        public MaterialsRequestNote_ItemService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<MaterialsRequestNote_Item>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<MaterialsRequestNote_Item> Query = this.DbContext.MaterialsRequestNote_Items;

            List<string> SearchAttributes = new List<string>()
                {
                    "Code"
                };
            Query = ConfigureSearch(Query, SearchAttributes, Keyword);

            List<string> SelectedFields = new List<string>()
                {
                    "Id", "Code"
                };
            Query = Query
                .Select(b => new MaterialsRequestNote_Item
                {
                    Id = b.Id,
                    Code = b.Code
                });

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = ConfigureOrder(Query, OrderDictionary);

            Pageable<MaterialsRequestNote_Item> pageable = new Pageable<MaterialsRequestNote_Item>(Query, Page - 1, Size);
            List<MaterialsRequestNote_Item> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public override void OnCreating(MaterialsRequestNote_Item model)
        {
            do
            {
                model.Code = CodeGenerator.GenerateCode();
            }
            while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

            base.OnCreating(model);
            var identity = ServiceProvider.GetService<IdentityService>();
            model._CreatedAgent = "Service";
            model._CreatedBy = identity.Username;
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = identity.Username;
        }

        public override void OnUpdating(int id, MaterialsRequestNote_Item model)
        {
            base.OnUpdating(id, model);
            model._LastModifiedAgent = "Service";
            model._LastModifiedBy = this.Username;
        }

        public override void OnDeleting(MaterialsRequestNote_Item model)
        {
            base.OnDeleting(model);
            model._DeletedAgent = "Service";
            model._DeletedBy = this.Username;
        }
    }
}
