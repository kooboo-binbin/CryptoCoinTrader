using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Exchanges.Gdax
{
    public class GdaxOrderStatusMapper : IGdaxOrderStatusMapper
    {
        private Lazy<Dictionary<string, OrderStatus>> _mapper = new Lazy<Dictionary<string, OrderStatus>>(() =>
        {
            return new Dictionary<string, OrderStatus>() { { "open", OrderStatus.Open }, { "pending", OrderStatus.Pending }, { "active", OrderStatus.Active } { "done", OrderStatus.Finished } };
        });

        private Lazy<Dictionary<OrderStatus, string>> _toStringMapper = new Lazy<Dictionary<OrderStatus, string>>(() =>
        {
            return new Dictionary<OrderStatus, string>() { { OrderStatus.Open, "open" }, { OrderStatus.Pending, "pending" }, { OrderStatus.Active, "active" }, { OrderStatus.Finished, "done" } };
        });

        public string GetName(OrderStatus orderStatus)
        {
            return _toStringMapper.Value.GetValueOrDefault(orderStatus, "open");
        }

        public OrderStatus GetOrderStatus(string name)
        {
            return _mapper.Value.GetValueOrDefault(name, OrderStatus.Unknowed);
        }
    }
}
