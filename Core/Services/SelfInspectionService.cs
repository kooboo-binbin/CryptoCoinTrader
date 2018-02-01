using System;
using System.Collections.Generic;
using System.Text;
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

        public SelfInspectionService(AppSettings appSettings,
            IBitstampConfig bitStampConfig,
            IGdaxConfig gdaxConfig,
            IObservationService observationService,
            IExchangeDataService exchangeDataService)
        {
            _appSettings = appSettings;
            _bitStampConfig = bitStampConfig;
            _gdaxConfig = gdaxConfig;
            _observationService = observationService;
            _exchangeDataService = exchangeDataService;
        }

        public MethodResult Inspect()
        {
            var messages = new List<string>();
            var funcs = new List<Func<string>>() { CheckIP, CheckBitStamp, CheckGdax, CheckObservations };
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
            var errorMessage = "Bit stamp config is not configured. please open bitstamp.json to fill it";
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
            var errorMessage = "Gdax config is not configured. please open bitstamp.json to fill it";
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
                return "observations.json is not configured,";
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

        private string CheckObservation(Data.Entities.Observation item)
        {
            var exchangeNames = _exchangeDataService.GetExchangeNames();
            var errorMessages = new List<string>();
            if (string.IsNullOrWhiteSpace(item.Exchange1Name))
            {
                errorMessages.Add("exchange1name should not be empty");
            }
            if (string.IsNullOrWhiteSpace(item.Exchange2Name))
            {
                errorMessages.Add("exchange2name should not be empty");
            }

            if (!exchangeNames.Contains(item.Exchange1Name))
            {
                errorMessages.Add($"{item.Exchange1Name} is invalid");
            }
            if (!exchangeNames.Contains(item.Exchange2Name))
            {
                errorMessages.Add($"{item.Exchange2Name} is invalid");
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
