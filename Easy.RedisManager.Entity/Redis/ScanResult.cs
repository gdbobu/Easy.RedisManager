using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Redis
{
    public class ScanResult
    {
        public ulong Cursor { get; set; }
        public List<byte[]> Results { get; set; }
    }
}
