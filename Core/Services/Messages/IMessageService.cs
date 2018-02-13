using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IMessageService
    {
        void Write(Guid observationId, string observationName, string message);
        void Error(Guid observationId, string observationName, string message);
        void Write(string message);
    }
}
