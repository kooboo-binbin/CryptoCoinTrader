using CryptoCoinTrader.Manifest;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface ISelfInspectionService
    {
        MethodResult Inspect();
    }
}
