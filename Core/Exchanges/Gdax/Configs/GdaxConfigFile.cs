using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Configs
{
    public class GdaxConfigFile : IGdaxConfig
    {
        public GdaxConfigInfo GetConfigInfo()
        {
            var file = "gdax.json";
            if (File.Exists(file))
            {
                using (var sr = new StreamReader(file))
                {
                    var json = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<GdaxConfigInfo>(json);
                }
            }
            else
            {
                using (var sw = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(new GdaxConfigInfo() { Key = "", Passphrase = "", Secret = "" }, Formatting.Indented);
                    sw.Write(json);
                }
                return null;
            }
        }
    }
}
