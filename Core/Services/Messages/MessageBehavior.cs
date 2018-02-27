using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CryptoCoinTrader.Core.Services.Messages
{
    public class MessageBehavior : WebSocketBehavior
    {
        public static MessageBehavior Instance;
        public MessageBehavior()
        {
            Instance = this;
        }

        protected override void OnMessage(MessageEventArgs e)
        {

        }

        public void SendMessage(string message)
        {
            Sessions.Broadcast(message);
        }
    }
}
