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

        private readonly ICoinContextService _coinContextService;

        public OrderService(CoinContext context, IMemoryCache cache,
            ICoinContextService coinContextService)
        {
            _coinContext = context;
            _cache = cache;
            _coinContextService = coinContextService;
        }

        public void Add(Order order1, Order order2)
        {
            _coinContextService.Execute((context) =>
            {
                context.Add(order1);
                context.Add(order2);
                context.SaveChanges();
            });
        }

        public List<Order> GetList(Guid arbitrageId)
        {
            return _cache.GetOrCreate(GetKey(arbitrageId), (ICacheEntry entry) =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(10);
                using (var context = _coinContextService.GetContext())
                {
                    return context.Orders.Where(it => it.ArbitrageId == arbitrageId).ToList();
                }
            });
        }

        public void UpdateStatus(Guid orderId, OrderStatus status)
        {
            _coinContextService.Execute((context) =>
            {
                var order = context.Orders.FirstOrDefault(it => it.Id == orderId);
                if (order != null)
                {
                    order.OrderStatus = status;
                    context.SaveChanges();
                    _cache.Remove(GetKey(order.ArbitrageId));
                }
            });
        }


        private string GetKey(Guid arbitrageId)
        {
            return $"orders:{arbitrageId}";
        }
    }
}
