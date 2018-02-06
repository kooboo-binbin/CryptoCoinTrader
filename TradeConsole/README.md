    We can't easy config some settings in console application. so all settings is saved in files.
    Before running you will see bitstamp.json, gdax.json and observations.json in the folder.
    if not.
    you should run the tradeconsole.dll first. 
    (How to run dotnet core application. https://www.microsoft.com/net/learn/get-started/macos)
    bitstamp.json is the settings file for bitstamp exchange.
    gdax.json is the settings file for gdax exchange.
    overvations.json is the settings file for arbitrages. 


How to use
1. dotnet tradeconsole.dll
2. in the folder you will see bitstamp.json and gdax.json. fill your informations.
3. there is a observations.json in the foloder too.  you should config it too. the id should be guid. 
4. run the app again  dotnet tradeconsole.dll

# observations.json 

```Java
  "Id": "dc814ec9-3b0d-42e1-a308-5f5556a4e5de",  /// it is id which could be any guid
    "BuyExchangeName": "gdax",					/// which exchange you wnat to buy
    "SellExchangeName": "bitstamp",				/// which exchange you want to sell
    "CurrencyPair": 0,							/// which curreny pair you want to trade, beteur is 0, ltceur is 1
    "SpreadValue": 3.0,							/// 
    "SpreadPercentage": 0.03,
    "SpreadType": 0,							///0 Arbitrage by spreadValue, 1 Arbitrage by Spread Percentage
    "SpreadMinimumVolume": 0.05,				///we only do a arbitrage if the volume is greater than we set on here
    "PerVolume": 0.02,							///How many volume we want to buy and sell
    "MaxVolume": 100.0,							///All volume we want to trade . (after we reach it, we will stop this task)
    "AvaialbeVolume": 85.71499556,				///How many volume are not trade.
    "RunningStatus": 0,							/// 0 is running you don' need to change it
    "DateCreated": "2018-02-01T11:50:39.5081391Z"   ///time.
    ```
