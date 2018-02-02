using CryptoCoinTrader.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTest
{
    public class BaseTest
    {
        protected DbContextOptions<CoinContext> GetOptions()
        {
            var builder = new DbContextOptionsBuilder<CoinContext>();
            builder.UseSqlite("Data Source=cointraderunittest.db");
            return builder.Options;
        }

        protected CoinContext GetContext()
        {
            var context = new CoinContext(GetOptions());
            context.Database.EnsureCreated();
            return context;
        }
    }
}
