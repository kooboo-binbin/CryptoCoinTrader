using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Helpers
{
    public static class TimeHelper
    {
        private static DateTime baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public static DateTime GetTime(long unixTimeStamp)
        {
            return baseDateTime.AddSeconds(unixTimeStamp);
        }
    }
}
