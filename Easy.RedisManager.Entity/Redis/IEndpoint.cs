using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Redis
{
    public interface IEndpoint
    {
        string Host { get; }
        int Port { get; }
    }
}
