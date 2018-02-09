using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IMessageService
    {
        void Write(Guid id, string message);
        void Error(Guid id, string message);
        void Write(string message);
    }
}
