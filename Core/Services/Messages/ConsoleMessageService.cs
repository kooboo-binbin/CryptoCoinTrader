using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Messages
{
    public class ConsoleMessageService : IMessageService
    {
        public void Write(Guid id, string observationName, string message)
        {
            Console.WriteLine(message);
        }

        public void Error(Guid id, string observationName, string message)
        {
            Write(id, observationName, message);
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
