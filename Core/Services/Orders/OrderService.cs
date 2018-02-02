using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IMemoryCache _cache;
        private readonly CoinContext _coinContext;
        private readonly object _lock = new object();

        public OrderService(CoinContext context, IMemoryCache cache)
        {
            _coinContext = context;
            _cache = cache;
        }

        public void Add(Order order1, Order order2)
        {
            lock (_lock)
            {
                _coinContext.Add(order1);
                _coinContext.Add(order2);
                _coinContext.SaveChanges();
            }
        }

        public List<Order> GetList(Guid arbitrageId)
        {
            return _cache.GetOrCreate(GetKey(arbitrageId), (ICacheEntry entry) =>
            {
                lock (_lock)
                {
                    entry.SlidingExpiration = TimeSpan.FromDays(10);
                    return _coinContext.Orders.Where(it => it.ArbitrageId == arbitrageId).ToList();
                }
            });
        }

        public void UpdateStatus(Guid orderId, OrderStatus status)
        {
            var order = _coinContext.Orders.FirstOrDefault(it => it.Id == orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                _coinContext.SaveChanges();
                _cache.Remove(GetKey(order.ArbitrageId));
            }
        }

        private string GetKey(Guid arbitrageId)
        {
            return $"orders:{arbitrageId}";
        }
    }
}
