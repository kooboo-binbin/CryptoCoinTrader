using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Trades;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public class ExchangeTradeService : IExchangeTradeService
    {
        
        public MethodResult<OrderResult> MakeANewOrder(string name, OrderRequest order)
        {
            throw new NotImplementedException();
        }
    }
}
