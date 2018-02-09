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
                if (string.IsNullOrWhiteSpace(config.ApiKey) || string.IsNullOrWhiteSpace(config.Secret) || string.IsNullOrWhiteSpace(config.CustomerId))
                {
                    return new MethodResult() { IsSuccessful = false, Message = errorMessage };
                }
            }
            return new MethodResult() { IsSuccessful = true };
        }
    }
}
