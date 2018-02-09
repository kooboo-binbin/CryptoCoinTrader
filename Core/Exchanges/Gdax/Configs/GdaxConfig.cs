using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Configs
{
    public class GdaxConfig : IGdaxConfig
    {
        private IExchangeSetting _exchangeSetting;

        public GdaxConfig(IExchangeSetting exchangeSetting)
        {
            _exchangeSetting = exchangeSetting;
        }

        public string Name => Constants.Name;

        public GdaxConfigInfo GetConfigInfo()
        {
            var settings = _exchangeSetting.GetSetting(this.Name);
            if (string.IsNullOrWhiteSpace(settings))
            {
                _exchangeSetting.SaveSetting(this.Name, GetDefaultJson());
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<GdaxConfigInfo>(settings);
            }
        }

        public string GetDefaultJson()
        {
            var json = JsonConvert.SerializeObject(new GdaxConfigInfo() { Key = "", Passphrase = "", Secret = "" }, Formatting.Indented);
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
                var config = JsonConvert.DeserializeObject<GdaxConfigInfo>(json);
                if (string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret) || string.IsNullOrWhiteSpace(config.Passphrase))
                {
                    return new MethodResult() { IsSuccessful = false, Message = errorMessage };
                }
            }
            return new MethodResult() { IsSuccessful = true };
        }
    }
}
