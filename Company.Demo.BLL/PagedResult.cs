using System;
using System.Collections.Generic;

namespace Company.Demo.BLL
{
    /// <summary>
    /// Generic paged result for server-side paging, sorting, and filtering.
    /// </summary>
    /// <typeparam name="T">The type of items in the result.</typeparam>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public string? SortField { get; set; }
        public bool SortAscending { get; set; } = true;
        public string? SearchQuery { get; set; }
    }
} 