using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoCoinTrader.Core.Services
{
    public class CoinContextService : ICoinContextService
    {
        public void Execute(Action<CoinContext> action)
        {
            using (var context = GetContext())
            {
                action(context);
            }
        }

        public CoinContext GetContext()
        {
            return new CoinContext(GetOptions());
        }

        private DbContextOptions<CoinContext> GetOptions()
        {
            var builder = new DbContextOptionsBuilder<CoinContext>();
            builder.UseSqlite("Data Source=cointrader.db");
            return builder.Options;
        }
    }
}
