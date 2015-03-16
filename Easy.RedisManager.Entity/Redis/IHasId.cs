using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Redis
{
    public interface IHasId<T>
    {
        T Id { get; }
    }
}
