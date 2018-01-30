using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Helpers
{
    public static class TimeHelper
    {
        private static DateTime baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        /// <summary>
        /// Get time by unix time stamp
        /// </summary>
        /// <param name="unixTimeStamp">Seconds</param>
        /// <returns></returns>
        public static DateTime GetTime(long unixTimeStamp)
        {
            return baseDateTime.AddSeconds(unixTimeStamp);
        }

        /// <summary>
        /// Get current time stamp
        /// </summary>
        /// <returns></returns>
        public static double GetTimeStamp()
        {
            return (DateTime.UtcNow - baseDateTime).TotalSeconds;
        }
    }
}
