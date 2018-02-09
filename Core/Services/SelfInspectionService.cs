using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Manifest;
using Newtonsoft.Json;
using RestSharp;

namespace CryptoCoinTrader.Core.Services
{
    public class SelfInspectionService : ISelfInspectionService
    {
        private readonly AppSettings _appSettings;
        private readonly IBitstampConfig _bitStampConfig;
        private readonly IGdaxConfig _gdaxConfig;
        private readonly IObservationService _observationService;
        private readonly IExchangeDataService _exchangeDataService;
        private readonly IExchangeTradeService _exchangeTradeService;

        public SelfInspectionService(AppSettings appSettings,
            IBitstampConfig bitStampConfig,
            IGdaxConfig gdaxConfig,
            IObservationService observationService,
            IExchangeDataService exchangeDataService,
            IExchangeTradeService exchangeTradeService)
        {
            _appSettings = appSettings;
            _bitStampConfig = bitStampConfig;
            _gdaxConfig = gdaxConfig;
            _observationService = observationService;
            _exchangeDataService = exchangeDataService;
            _exchangeTradeService = exchangeTradeService;
        }

        public MethodResult Inspect()
        {
            var messages = new List<string>();
            var funcs = new List<Func<string>>() { CheckIP, CheckBitStamp, CheckGdax, CheckExchanges, CheckObservations };
            foreach (var fun in funcs)
            {
                var message = fun();
                if (!string.IsNullOrEmpty(message))
                {
                    messages.Add(message);
                }
            }
            if (messages.Count > 0)
            {
                return new MethodResult() { IsSuccessful = false, Message = string.Join("\r\n", messages) };
            }
            return new MethodResult() { IsSuccessful = true };
        }

        /// <summary>
        /// Check the whether the IP is correct
        /// </summary>
        /// <returns></returns>
        private string CheckIP()
        {
            var client = new RestClient("https://api.ipify.org");
            var request = new RestRequest("/", Method.GET);
            request.AddQueryParameter("format", "json");
            var response = client.Execute<IPInfo>(request);
            if (!_appSettings.WorkingIps.Contains(response.Data.ip))
            {
                return "IP is incorrect. please use vpn or change the working ips in appsetings.json";
            }
            return string.Empty;
        }

        private string CheckBitStamp()
        {
            var errorMessage = "Bit stamp config is not configured.";
            var config = _bitStampConfig.GetConfigInfo();
            if (config == null)
            {
                return errorMessage;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(config.ApiKey) || string.IsNullOrWhiteSpace(config.Secret) || string.IsNullOrWhiteSpace(config.CustomerId))
                {
                    return errorMessage;
                }
            }
            return string.Empty;
        }

        private string CheckGdax()
        {
            var errorMessage = "Gdax config is not configured. ";
            var config = _gdaxConfig.GetConfigInfo();
            if (config == null)
            {
                return errorMessage;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret) || string.IsNullOrWhiteSpace(config.Passphrase))
                {
                    return errorMessage;
                }
            }
            return string.Empty;
        }

        private string CheckObservations()
        {
            var observations = _observationService.GetObservations();
            if (observations == null || observations.Count == 0)
            {
                return "observations is empty, please add a least one";
            }
            else
            {
                var errorMessages = new List<string>();
                foreach (var item in observations)
                {
                    var result = CheckObservation(item);
                    if (!string.IsNullOrEmpty(result))
                    {
                        errorMessages.Add(result);
                    }
                }
                if (errorMessages.Count > 0)
                {
                    return string.Join("\r\n", errorMessages);
                }
            }
            return string.Empty;
        }

        private string CheckExchanges()
        {
            var errorMessages = new List<string>();
            var dataNames = _exchangeDataService.GetExchangeNames().Select(it => it.ToLower()).ToList();
            var tradeNames = _exchangeTradeService.GetExchangeNames().Select(it => it.ToLower()).ToList();
            var temp = dataNames.Except(tradeNames);
            if (temp.Count() != 0)
            {
                return "exchange data and trade has some different names, please ask exchange developer.";
            }
            return string.Empty;
        }

        private string CheckObservation(Data.Entities.Observation item)
        {
            var exchangeNames = _exchangeDataService.GetExchangeNames();
            var errorMessages = new List<string>();
            if (string.IsNullOrWhiteSpace(item.BuyExchangeName))
            {
                errorMessages.Add("BuyExchangeName should not be empty");
            }
            if (string.IsNullOrWhiteSpace(item.SellExchangeName))
            {
                errorMessages.Add("SellExchangeName should not be empty");
            }

            if (!exchangeNames.Contains(item.BuyExchangeName))
            {
                errorMessages.Add($"{item.BuyExchangeName} is invalid");
            }
            if (!exchangeNames.Contains(item.SellExchangeName))
            {
                errorMessages.Add($"{item.SellExchangeName} is invalid");
            }
            if (item.BuyExchangeName.ToLower() == item.SellExchangeName.ToLower())
            {
                errorMessages.Add("sellExchangeName should be different from buyExchangeName");
            }
            if (errorMessages.Count > 0)
            {
                return string.Join(", ", errorMessages);
            }
            return string.Empty;
        }


        private class IPInfo
        {
            public string ip { get; set; }
        }
    }
}
