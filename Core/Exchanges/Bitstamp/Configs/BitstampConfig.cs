using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs
{
    public class BitstampConfig : IBitstampConfig
    {
        private IExchangeSetting _exchangeSetting;

        public BitstampConfig(IExchangeSetting exchangeSetting)
        {
            _exchangeSetting = exchangeSetting;
        }

        public string Name => Constants.Name;

        public BitstampConfigInfo GetConfigInfo()
        {
            var settings = _exchangeSetting.GetSetting(this.Name);
            if (string.IsNullOrWhiteSpace(settings))
            {
                _exchangeSetting.SaveSetting(this.Name, GetDefaultJson());
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<BitstampConfigInfo>(settings);
            }

        }

        public string GetDefaultJson()
        {
            var json = JsonConvert.SerializeObject(new BitstampConfigInfo() { ApiKey = "", CustomerId = "", Secret = "" }, Formatting.Indented);
            return json;
        }

        public MethodResult Valid(string json)
        {
            var errorMessage = "Gdax config is not configured. ";

            if (string.IsNullOrWhiteSpace(json))
            {
                return new MethodResult() { IsSuccessful = false, Message = errorMessage };
            }
            else
            {
                var config = JsonConvert.DeserializeObject<BitstampConfigInfo>(json);
                var messages = new List<string>();
                if (string.IsNullOrWhiteSpace(config.ApiKey))
                {
                    messages.Add("ApiKey is empty");
                }
                if (string.IsNullOrWhiteSpace(config.Secret))
                {
                    messages.Add("Secret is empty");
                }
                if (string.IsNullOrWhiteSpace(config.CustomerId))
                {
                    messages.Add("CustomerId is empty");
                }
                if (messages.Count > 0)
                {
                    return new MethodResult() { IsSuccessful = false, Message = string.Join("<br />", messages) };
                }
            }
            return new MethodResult() { IsSuccessful = true };
        }
    }
}
