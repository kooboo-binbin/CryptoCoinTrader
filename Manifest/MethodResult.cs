using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest
{
    public class MethodResult<T> where T : class
    {
        public T Data { get; set; }
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }
    }

    public class MethodResult
    {
        public static MethodResult<T> Success<T>(T data) where T : class
        {
            return new MethodResult<T>() { Data = data };
        }

        public static MethodResult<T> Failed<T>(string message) where T : class
        {
            return new MethodResult<T>() { IsSuccessful = false, Message = message };
        }
    }
}
