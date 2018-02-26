using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bl3p.Infos;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Interfaces;
using Newtonsoft.Json;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p.Configs
{
    public class Bl3pConfig : IBl3pConfig
    {
        private IExchangeSetting _exchangeSetting;
        public Bl3pConfig(IExchangeSetting exchangeSetting)
        {
            _exchangeSetting = exchangeSetting;
        }

        public string Name => Constants.Name;


        public Bl3pConfigInfo GetConfigInfo()
        {
            var settings = _exchangeSetting.GetSetting(this.Name);
            if (string.IsNullOrWhiteSpace(settings))
            {
                _exchangeSetting.SaveSetting(this.Name, GetDefaultJson());
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<Bl3pConfigInfo>(settings);
            }
        }

        public string GetDefaultJson()
        {
            var json = JsonConvert.SerializeObject(new Bl3pConfigInfo() { PublicKey = "", PrivateKey = "" }, Formatting.Indented);
            return json;
        }

        public MethodResult Valid(string json)
        {
            var errorMessage = "Bl3p config is not configured. ";

            if (string.IsNullOrWhiteSpace(json))
            {
                return new MethodResult() { IsSuccessful = false, Message = errorMessage };
            }
            else
            {
                var config = JsonConvert.DeserializeObject<Bl3pConfigInfo>(json);
                var messages = new List<string>();
                if (string.IsNullOrWhiteSpace(config.PublicKey))
                {
                    messages.Add("PublicKey is empty");
                }
                if (string.IsNullOrWhiteSpace(config.PrivateKey))
                {
                    messages.Add("PrivateKey is empty");
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
