using System;
using System.Collections.Generic;
using System.Text;
using WebSocket4Net;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{
    public class Bl3pWebSocket : WebSocket
    {
        public string Url { get; set; }

        public Bl3pWebSocket(string url) : base(url)
        {
            this.Url = url;
        }
    }
}
