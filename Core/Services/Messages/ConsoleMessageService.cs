using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Messages
{
    public class ConsoleMessageService : IMessageService
    {
        private object _consoleLock = new object();
        public void Write(int top, string message)
        {
            lock (_consoleLock)
            {
                Console.SetCursorPosition(0, top);
                Console.WriteLine(message);
            }
        }
    }
}
