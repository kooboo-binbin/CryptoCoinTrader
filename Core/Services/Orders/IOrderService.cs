using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Orders
{
    public interface IOrderService
    {
        void Add(Order order1, Order order2);

        void UpdateStatus(Guid orderId, OrderStatus status);

        /// <summary>
        /// the order should be stored in cache
        /// </summary>
        /// <param name="arbitrageId"></param>
        /// <returns></returns>
        List<Data.Entities.Order> GetList(Guid arbitrageId);

       

    }
}
