using System;
using System.Collections.Generic;
using System.Text;
using WebSocket4Net;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{
    public class Bl3pWebSocket : WebSocket
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public Bl3pWebSocket(string name, string url) : base(url)
        {
            this.Name = name;
            this.Url = url;
        }
    }
}
