using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Orders
{
    public interface IOrderService
    { 
        void Add(Data.Entities.Order order);
        void Update(Data.Entities.Order order);
    }
}
