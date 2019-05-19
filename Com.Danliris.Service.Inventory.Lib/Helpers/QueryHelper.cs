﻿using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Helpers
{
    public static class QueryHelper<TModel>
         where TModel : StandardEntity
    {
        public static IQueryable<TModel> ConfigureSearch(IQueryable<TModel> Query, List<string> SearchAttributes, string Keyword, bool ToLowerCase = false, string SearchWith = "Contains")
        {
            /* Search with Keyword */
            if (Keyword != null)
            {
                string SearchQuery = String.Empty;
                foreach (string Attribute in SearchAttributes)
                {
                    if (Attribute.Contains("."))
                    {
                        var Key = Attribute.Split(".");
                        SearchQuery = string.Concat(SearchQuery, Key[0], $".Any({Key[1]}.", SearchWith, "(@0)) OR ");
                    }
                    else
                    {
                        SearchQuery = string.Concat(SearchQuery, Attribute, ".", SearchWith, "(@0) OR ");
                    }
                }

                SearchQuery = SearchQuery.Remove(SearchQuery.Length - 4);

                if (ToLowerCase)
                {
                    SearchQuery = SearchQuery.Replace("." + SearchWith + "(@0)", ".ToLower()." + SearchWith + "(@0)");
                    Keyword = Keyword.ToLower();
                }

                Query = Query.Where(SearchQuery, Keyword);
            }
            return Query;
        }

        public static IQueryable<TModel> ConfigureFilter(IQueryable<TModel> Query, Dictionary<string, string> FilterDictionary)
        {
            if (FilterDictionary != null && !FilterDictionary.Count.Equals(0))
            {
                foreach (var f in FilterDictionary)
                {
                    string Key = f.Key;
                    object Value = f.Value;
                    bool ParsedValueBoolean;

                    string filterQuery = string.Concat(string.Empty, Key, " == @0");

                    if (Boolean.TryParse(Value.ToString(), out ParsedValueBoolean))
                    {
                        Query = Query.Where(filterQuery, ParsedValueBoolean);
                    }
                    else
                    {
                        Query = Query.Where(filterQuery, Value);
                    }
                }
            }
            return Query;
        }

        public static IQueryable<TModel> ConfigureOrder(IQueryable<TModel> Query, Dictionary<string, string> OrderDictionary)
        {
            /* Default Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_lastModifiedUtc", "desc");

                Query = Query.OrderBy("_lastModifiedUtc desc");
            }
            /* Custom Order */
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key.Replace(".", ""), " ", OrderType));
            }
            return Query;
        }

        public static IQueryable ConfigureSelect(IQueryable<TModel> Query, Dictionary<string, string> SelectDictionary)
        {
            /* Custom Select */
            if (SelectDictionary != null && !SelectDictionary.Count.Equals(0))
            {
                string selectedColumns = string.Join(", ", SelectDictionary.Select(d => (d.Value == "1") ? d.Key : string.Concat(d.Value, " as ", d.Key)));

                var SelectedQuery = Query.Select(string.Concat("new(", selectedColumns, ")"));

                return SelectedQuery;
            }

            /* Default Select */
            return Query;
        }
    }
}
