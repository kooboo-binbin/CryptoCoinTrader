using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Messages
{
    public class MessageService : IMessageService
    {
        private ICoinContextService _coinContextService;
        public MessageService(ICoinContextService coinContextService)
        {
            _coinContextService = coinContextService;
        }

        public void Write(Guid guid, string observationName, string message)
        {
            AddLog(guid, observationName, message);
            SendToWS(LogType.Info, message);
            Console.WriteLine(message);
        }

        public void Error(Guid id, string observationName, string message)
        {
            AddLog(id, observationName, message, LogType.Error);
            SendToWS(LogType.Error, message);
            Console.WriteLine(message);
        }

        public void Write(string message)
        {
            AddLog(null, null, message);
            SendToWS(LogType.Info, message);
            Console.WriteLine(message);
        }

        private void AddLog(Guid? guid, string observationName, string message, LogType type = LogType.Info)
        {
            using (var context = _coinContextService.GetContext())
            {
                var log = new Log();
                log.DateCreated = DateTime.UtcNow;
                log.LogType = type;
                log.Message = message;
                log.ObservatoinId = guid;
                log.ObservationName = observationName;
                context.Add(log);
                context.SaveChanges();
            }
        }

        private void SendToWS(LogType type, string message)
        {
            try
            {
                var info = new LogInfo() { Type = type, Message = message };
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var json = JsonConvert.SerializeObject(info, serializerSettings);
                MessageBehavior.Instance?.SendMessage(json);
            }
            catch (Exception ex)
            {
            }
        }

        private class LogInfo
        {
            public LogType Type { get; set; }
            public string Message { get; set; }
        }
    }
}
