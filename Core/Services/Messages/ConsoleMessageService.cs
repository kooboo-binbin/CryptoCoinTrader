using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Messages
{
    public class ConsoleMessageService : IMessageService
    {
        public void Write(Guid guid, string message)
        {
            Console.WriteLine(message);
        }

        public void Error(Guid id, string message)
        {
            Write(id, message);
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
