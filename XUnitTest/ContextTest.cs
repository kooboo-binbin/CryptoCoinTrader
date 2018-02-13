using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace XUnitTest
{
    public class ContextTest : BaseTest
    {
        [Fact]
        public void Many()
        {
            var context1 = GetContext();
            context1.Database.EnsureCreated();
            Parallel.For(0, 10, (int k) =>
            {
                var context = GetContext();

                var log = new Log();
                log.DateCreated = DateTime.UtcNow;
                log.Message = k.ToString();
                log.ObservationName = "aa" + k;
                context.Add(log);
                context.SaveChanges();
                var logs = context.Logs.ToList();
                var kk = logs.FirstOrDefault();
            });
        }

        [Fact]
        public void Many2()
        {
            //coincontext is not thread safe. it will report an exception. 
            //A second operation started on this context before a previous operation completed
            //var context = GetContext();
            //context.Database.EnsureCreated();
            //Parallel.For(0, 10, (int k) =>
            //{
            //    var log = new Log();
            //    log.DateCreated = DateTime.UtcNow;
            //    log.Message = k.ToString();
            //    log.ObservationName = "aa" + k;
            //    context.Add(log);
            //    context.SaveChanges();
            //    var logs = context.Logs.ToList();
            //    var kk = logs.FirstOrDefault();
            //});
        }
    }
}
