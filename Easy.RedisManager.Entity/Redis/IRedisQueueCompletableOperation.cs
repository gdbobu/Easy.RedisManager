﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.RedisManager.Entity.Redis
{
    public interface IRedisQueueCompletableOperation
    {
        void CompleteVoidQueuedCommand(Action voidReadCommand);
        void CompleteIntQueuedCommand(Func<int> intReadCommand);
        void CompleteLongQueuedCommand(Func<long> longReadCommand);
        void CompleteBytesQueuedCommand(Func<byte[]> bytesReadCommand);
        void CompleteMultiBytesQueuedCommand(Func<byte[][]> multiBytesReadCommand);
        void CompleteStringQueuedCommand(Func<string> stringReadCommand);
        void CompleteMultiStringQueuedCommand(Func<List<string>> multiStringReadCommand);
        void CompleteDoubleQueuedCommand(Func<double> doubleReadCommand);
    }
}
