using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{
    public class BitstampOrderStatusMapper : IBitmapOrderStatusMapper
    {
        private Lazy<Dictionary<string, OrderStatus>> _mapper = new Lazy<Dictionary<string, OrderStatus>>(() =>
        {
            return new Dictionary<string, OrderStatus>() { { "queue", OrderStatus.Pending }, { "open", OrderStatus.Open }, { "finished", OrderStatus.Finished } };
        });

        private Lazy<Dictionary<OrderStatus, string>> _toStringMapper = new Lazy<Dictionary<OrderStatus, string>>(() =>
        {
            return new Dictionary<OrderStatus, string>() { { OrderStatus.Finished, "Finished" }, { OrderStatus.Open, "Open" }, { OrderStatus.Pending, "queue" } };
        });

        public string GetName(OrderStatus orderStatus)
        {
            return _toStringMapper.Value.GetValueOrDefault(orderStatus, "Open");
        }

        public OrderStatus GetOrderStatus(string name)
        {
            return _mapper.Value.GetValueOrDefault(name, OrderStatus.Unknowed);
        }
    }
}
