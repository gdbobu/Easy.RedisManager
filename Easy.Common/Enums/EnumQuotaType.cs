using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    public enum EnumQuotaType
    {
        Operations,      //ServiceStack
        Types,           //Text, Redis
        Fields,          //ServiceStack, Text, Redis, OrmLite
        RequestsPerHour, //Redis
        Tables,          //OrmLite
        PremiumFeature,  //AdminUI, Advanced Redis APIs, etc
    }
}
