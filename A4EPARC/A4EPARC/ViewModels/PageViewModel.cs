using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using A4EPARC.Extensions;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace A4EPARC.ViewModels
{
    public class PagedViewModel<T>
    {
        public ViewDataDictionary ViewData { get; set; }
        public IQueryable<T> Query { get; set; }
        public GridSortOptions GridSortOptions { get; set; }
        public string DefaultSortColumn { get; set; }
        public IPagination<T> PagedList { get; private set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }
        public int? LastPage { get; set; }
        public int? FirstPage { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public bool ShowAllRecords { get; set; }
        public List<string> EnvironemntTypeList { get; set; }
        
        public PagedViewModel<T> AddFilter(Expression<Func<T, bool>> predicate)
        {
            Query = Query.Where(predicate);
            return this;
        }

        public PagedViewModel<T> AddFilter<TValue>(string key, TValue value, Expression<Func<T, bool>> predicate)
        {
            ProcessQuery(value, predicate);
            ViewData[key] = value;
            return this;
        }

        public PagedViewModel<T> AddFilter<TValue>(string keyField, object value, Expression<Func<T, bool>> predicate,
            IQueryable<TValue> query, string textField)
        {
            ProcessQuery(value, predicate);
            var selectList = query.ToSelectList(keyField, textField, value);
            ViewData[keyField] = selectList;
            return this;
        }

        public PagedViewModel<T> Setup()
        {
            if (string.IsNullOrWhiteSpace(GridSortOptions.Column))
            {
                GridSortOptions.Column = DefaultSortColumn;
                if (DefaultSortColumn.ToUpper() == "CREATEDDATE") 
                {
                    GridSortOptions.Direction = SortDirection.Descending;
                }
            }

            PagedList = Query.OrderBy(GridSortOptions.Column, GridSortOptions.Direction)
                .AsPagination(CurrentPage ?? 1, PageSize ?? 10);
            return this;
        }

        private void ProcessQuery<TValue>(TValue value, Expression<Func<T, bool>> predicate)
        {
            if (value == null) return;
            if (typeof(TValue) == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(value as string)) return;
            }

            Query = Query.Where(predicate);
        }
    }
}