using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest
{
    public class MethodResult<T> : MethodResult
    {
        public T Data { get; set; }

        public static MethodResult<T> Failed(string message)
        {
            return new MethodResult<T>() { IsSuccessful = false, Message = message };
        }
    }

    public class MethodResult
    {
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }
    }
}
