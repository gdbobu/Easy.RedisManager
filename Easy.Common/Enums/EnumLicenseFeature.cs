using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    [Flags]
    public enum EnumLicenseFeature : long
    {
        None = 0,
        All = Premium | Text | Client | Common | Redis | OrmLite | ServiceStack | Server | Razor | Admin,
        RedisSku = Redis | Text,
        OrmLiteSku = OrmLite | Text,
        Free = None,
        Premium = 1 << 0,
        Text = 1 << 1,
        Client = 1 << 2,
        Common = 1 << 3,
        Redis = 1 << 4,
        OrmLite = 1 << 5,
        ServiceStack = 1 << 6,
        Server = 1 << 7,
        Razor = 1 << 8,
        Admin = 1 << 9,
    }
}
