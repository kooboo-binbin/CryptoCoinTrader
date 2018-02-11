using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Models
{
    /// <summary>
    /// Page model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageModel<T> where T : class
    {
        /// <summary>
        /// constructor
        /// </summary>
        public PageModel()
        {
            Pagination = new Pagination();
        }

        /// <summary>
        /// Items
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Pagination
        /// </summary>
        public Pagination Pagination { get; set; }
    }

    /// <summary>
    /// Pagination
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Current page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }


        public int PageCount { get; set; }

        public int Total { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}
