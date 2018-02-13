using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Messages
{
    public class MessageService : IMessageService
    {
        private CoinContext _coinContext;
        public MessageService(CoinContext coinContext)
        {
            _coinContext = coinContext;
        }

        public void Write(Guid guid, string observationName, string message)
        {
            AddLog(guid, observationName, message);
            Console.WriteLine(message);
        }

        public void Error(Guid id, string observationName, string message)
        {
            AddLog(id, observationName, message, LogType.Error);
            Console.WriteLine(message);
        }

        public void Write(string message)
        {
            AddLog(null, null, message);
            Console.WriteLine(message);
        }

        private void AddLog(Guid? guid, string observationName, string message, LogType type = LogType.Info)
        {
            var log = new Log();
            log.DateCreated = DateTime.UtcNow;
            log.LogType = type;
            log.Message = message;
            log.ObservatoinId = guid;
            log.ObservationName = observationName;
            _coinContext.Add(log);
            _coinContext.SaveChanges();
        }
    }
}
