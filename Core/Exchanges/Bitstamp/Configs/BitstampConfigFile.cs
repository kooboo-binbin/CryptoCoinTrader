using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs
{
    public class BitstampConfigFile : IBitstampConfig
    {
        public BitstampConfigInfo GetConfigInfo()
        {
            var file = "bitstamp.json";
            if (File.Exists(file))
            {
                using (var sr = new StreamReader(file))
                {
                    var json = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<BitstampConfigInfo>(json);
                }
            }
            else
            {
                using (var sw = new StreamWriter(file))
                {
                    var json = JsonConvert.SerializeObject(new BitstampConfigInfo() { ApiKey = "", CustomerId = "", Secret = "" }, Formatting.Indented);
                    sw.Write(json);
                }
                return null;
            }
        }
    }
}
