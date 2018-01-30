using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CryptoCoinTrader.Manifest.Helpers
{
    public static class DecimalHelper
    {
        private static CultureInfo _enCulture = new CultureInfo("en-us");
        public static decimal Get(string input)
        {
            return decimal.Parse(input, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, _enCulture);
        }
    }
}
