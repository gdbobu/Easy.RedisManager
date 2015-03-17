using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Easy.Common.Pool
{
    /// <summary>
    /// Buffer池，用于集中管控Socket缓冲区，防止内存碎片。
    /// </summary>
    public class BufferPool
    {
        // Pool的大小
        private const int PoolSize = 1000;      // 1.45MB
        // 每个缓存的大小
        public const int BufferLength = 1450;   // MTU size - some headers
        // Pool
        private static readonly object[] SPool = new object[PoolSize];

        private BufferPool() { }

        /// <summary>
        /// 清空Pool
        /// </summary>
        public static void Flush()
        {
            for (int i = 0; i < SPool.Length; i++)
            {
                // 以原子操作的形式，将对象设置为指定的值并返回对原始对象的引用。
                // location1  类型：System.Object  要设置为指定值的变量。 
                // value      类型：System.Object  location1 参数被设置为的值。 
                // 返回值     类型：System.Object  location1 的原始值。
                Interlocked.Exchange(ref SPool[i], null); // and drop the old value on the floor
            }
        }

        /// <summary>
        /// 获取Buffer
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBuffer()
        {
            object tmp;
            for (int i = 0; i < SPool.Length; i++)
            {
                if ((tmp = Interlocked.Exchange(ref SPool[i], null)) != null)
                    return (byte[])tmp;
            }
            return new byte[BufferLength];
        }

        /// <summary>
        /// 重新分配Buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="toFitAtLeastBytes"></param>
        /// <param name="copyFromIndex"></param>
        /// <param name="copyBytes"></param>
        public static void ResizeAndFlushLeft(ref byte[] buffer, int toFitAtLeastBytes, int copyFromIndex, int copyBytes)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(toFitAtLeastBytes > buffer.Length);
            Debug.Assert(copyFromIndex >= 0);
            Debug.Assert(copyBytes >= 0);

            // try doubling, else match
            int newLength = buffer.Length * 2;
            if (newLength < toFitAtLeastBytes) newLength = toFitAtLeastBytes;

            var newBuffer = new byte[newLength];
            if (copyBytes > 0)
            {
                Buffer.BlockCopy(buffer, copyFromIndex, newBuffer, 0, copyBytes);
            }
            if (buffer.Length == BufferLength)
            {
                ReleaseBufferToPool(ref buffer);
            }
            buffer = newBuffer;
        }

        /// <summary>
        /// 释放Buffer到Pool
        /// </summary>
        /// <param name="buffer"></param>
        public static void ReleaseBufferToPool(ref byte[] buffer)
        {
            if (buffer == null) return;
            if (buffer.Length == BufferLength)
            {
                for (int i = 0; i < SPool.Length; i++)
                {
                    // 比较两个对象是否相等，如果相等，则替换其中一个对象。
                    // location1 类型：System.Object   其值与 comparand 进行比较并且可能被替换的目标对象。 
                    // value     类型：System.Object   在比较结果相等时替换目标对象的对象。 
                    // comparand 类型：System.Object   与位于 location1 处的对象进行比较的对象。 
                    // 返回值    类型：System.Object   location1 中的原始值。
                    if (Interlocked.CompareExchange(ref SPool[i], buffer, null) == null)
                    {
                        break; // found a null; swapped it in
                    }
                }
            }
            // if no space, just drop it on the floor
            buffer = null;
        }
    }
}
