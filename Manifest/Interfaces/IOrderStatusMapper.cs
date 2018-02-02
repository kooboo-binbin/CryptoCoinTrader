using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IOrderStatusMapper
    {
        string GetName(OrderStatus orderStatus);
        OrderStatus GetOrderStatus(string name);
    }
}
