using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">the page is start from 1</param>
        /// <param name="pageSize"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        protected IQueryable<T> Paging<T>(IQueryable<T> query, int page, int pageSize, Pagination pagination) where T : class
        {
            var total = query.Count();
            var pageCount = (int)decimal.Ceiling(total / (decimal)pageSize);
            pagination.Total = total;
            pagination.Page = page;
            pagination.PageSize = pageSize;
            pagination.PageCount = pageCount;
            pagination.HasNextPage = page < pageCount;
            pagination.HasPreviousPage = page > 1;
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }

        protected void Paging<T>(IQueryable<T> query, int page, int pageSize, PageModel<T> model) where T : class
        {
            var temp = Paging(query, page, pageSize, model.Pagination);
            model.Items = temp.ToList();
        }
    }
}
