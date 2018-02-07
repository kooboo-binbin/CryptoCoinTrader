using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IMessageService
    {
        void Write(int position, string message);
        void Error(int position, string message);
    }
}
