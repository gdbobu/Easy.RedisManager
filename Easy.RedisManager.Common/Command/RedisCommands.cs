using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easy.Common;

namespace Easy.RedisManager.Common.Command
{
    /// <summary>
    /// The define of redis command
    /// http://redis.readthedocs.org/en/latest/index.html
    /// http://www.redis.cn/commands.html
    /// http://www.yiibai.com/redis/
    /// </summary>
    public class RedisCommands
    {
        #region Connection（连接）
        /// <summary>
        /// 请求服务器关闭连接。连接将会尽可能快的将未完成的客户端请求完成处理。
        /// 返回值：始终返回 OK.
        /// http://www.redis.cn/commands/quit.html
        /// http://redis.readthedocs.org/en/latest/connection/quit.html
        /// </summary>
        public readonly static byte[] Quit = "QUIT".ToUtf8Bytes();
        /// <summary>
        /// 为redis服务请求设置一个密码。redis可以设置在客户端执行commands请求前需要通过密码验证。
        /// 通过修改配置文件的requirepass就可以设置密码。 如果密码与配置文件里面设置的密码一致，
        /// 服务端就会发会一个OK的状态码，接受客户端发送其他的请求命令，否则服务端会返回一个错误码，
        /// 客户端需要尝试使用新的密码来进行连接。
        /// 
        /// 注意: 因为redis的高性能能在短时间接受非常多的尝试性密码，
        /// 所以请务必设置一个足够复杂的密码以防止可能的攻击。
        /// http://www.redis.cn/commands/auth.html
        /// http://redis.readthedocs.org/en/latest/connection/auth.html
        /// </summary>
        public readonly static byte[] Auth = "AUTH".ToUtf8Bytes();
        /// <summary>
        /// 返回 PONG。此命令通常是用来测试如果连接还活着，或延迟来衡量。
        /// 返回值：PONG
        /// http://www.redis.cn/commands/ping.html
        /// http://redis.readthedocs.org/en/latest/connection/ping.html
        /// </summary>
        public readonly static byte[] Ping = "PING".ToUtf8Bytes();
        /// <summary>
        /// 返回消息
        /// 返回值：
        /// Bulk reply
        /// http://www.redis.cn/commands/echo.html
        /// http://redis.readthedocs.org/en/latest/connection/echo.html
        /// </summary>
        public readonly static byte[] Echo = "ECHO".ToUtf8Bytes();
        /// <summary>
        /// 选择一个数据库，下标值从0开始，一个新连接默认连接的数据库是DB0。
        /// 返回值：
        /// Status code reply
        /// http://www.redis.cn/commands/select.html
        /// http://redis.readthedocs.org/en/latest/connection/select.html
        /// </summary>
        public readonly static byte[] Select = "SELECT".ToUtf8Bytes(); 
        #endregion

        #region Key（键）
        /// <summary>
        /// 返回key是否存在。
        /// 整数，如下的整数结果
        /// 1 如果key存在
        /// 0 如果key不存在
        /// http://www.redis.cn/commands/exists.html
        /// http://redis.readthedocs.org/en/latest/key/exists.html
        /// </summary>
        public readonly static byte[] Exists = "EXISTS".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(n)N为要移除的key的数量。
        /// 移除单个字符串类型的key，时间复杂度为O(1)。
        /// 移除单个列表、集合、有序集合或哈希表类型的key，时间复杂度为O(M)，
        /// M为以上数据结构内的元素数量。
        /// 如果删除的key不存在，则直接忽略。
        /// 返回值:被删除的keys的数量
        /// http://www.redis.cn/commands/del.html
        /// http://redis.readthedocs.org/en/latest/key/del.html
        /// </summary>
        public readonly static byte[] Del = "DEL".ToUtf8Bytes();
        /// <summary>
        /// 返回 key 所储存的值的类型。
        /// 时间复杂度： O(1)。
        /// 返回 key 所储存的值的类型。
        /// 返回值:none (key不存在)
        /// string (字符串)
        /// list (列表)
        /// set (集合)
        /// zset (有序集)
        /// hash (哈希表)
        /// http://www.redis.cn/commands/type.html
        /// http://redis.readthedocs.org/en/latest/key/type.html
        /// </summary>
        public readonly static byte[] Type = "TYPE".ToUtf8Bytes();
        /// <summary>
        /// 查找所有符合给定模式 pattern 的 key 。
        /// KEYS * 匹配数据库中所有 key 。
        /// KEYS h?llo 匹配 hello ， hallo 和 hxllo 等。
        /// KEYS h*llo 匹配 hllo 和 heeeeello 等。
        /// KEYS h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo 。
        /// 特殊符号用 \ 隔开。
        /// Warning：KEYS 的速度非常快，但在一个大的数据库中使用它仍然可能造成性能问题，
        /// 如果你需要从一个数据集中查找特定的 KEYS， 你最好还是用 Redis 的集合结构 SETS 来代替。
        /// 时间复杂度： O(N)，N 为数据库中 key 的数量。
        /// 返回值：符合给定模式 pattern 的 key 列表。
        /// http://www.redis.cn/commands/keys.html
        /// http://redis.readthedocs.org/en/latest/key/keys.html
        /// </summary>
        public readonly static byte[] Keys = "KEYS".ToUtf8Bytes();
        /// <summary>
        /// 从当前数据库返回一个随机的key。
        /// 时间复杂度： O(1)。
        /// 返回值:Bulk reply:如果数据库没有任何key，返回nil，否则返回一个随机的key。
        /// http://www.redis.cn/commands/randomkey.html
        /// http://redis.readthedocs.org/en/latest/key/randomkey.html
        /// </summary>
        public readonly static byte[] RandomKey = "RANDOMKEY".ToUtf8Bytes();
        /// <summary>
        /// 将key重命名为newkey，如果key与newkey相同，将返回一个错误。如果newkey已经存在，则值将被覆盖。
        /// 时间复杂度： O(1)。
        /// 返回值:Status code reply
        /// http://www.redis.cn/commands/rename.html
        /// http://redis.readthedocs.org/en/latest/key/rename.html
        /// </summary>
        public readonly static byte[] Rename = "RENAME".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// 当且仅当 newkey 不存在时，将 key 改名为 newkey 。
        /// 当 key 不存在时，返回一个错误。
        /// 返回值:
        /// Integer reply, specifically:
        /// 修改成功时，返回 1 。
        /// 如果 newkey 已经存在，返回 0 。
        /// http://www.redis.cn/commands/renamenx.html
        /// http://redis.readthedocs.org/en/latest/key/renamenx.html
        /// </summary>
        public readonly static byte[] RenameNx = "RENAMENX".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// 这个命令和 EXPIRE 命令的作用类似，但是它以毫秒为单位设置 key 的生存时间，而不像 EXPIRE 命令那样，以秒为单位。
        /// 返回值:
        /// Integer reply, specifically:
        /// 设置成功，返回 1
        /// key 不存在或设置失败，返回 0
        /// http://www.redis.cn/commands/pexpire.html
        /// http://redis.readthedocs.org/en/latest/key/pexpire.html
        /// </summary>
        public readonly static byte[] PExpire = "PEXPIRE".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// PEXPIREAT 这个命令和 EXPIREAT命令类似，但它以毫秒为单位设置 key 的过期 unix 时间戳，而不是像 EXPIREAT 那样，以秒为单位。
        /// 返回值:
        /// Integer reply, specifically:
        /// 如果生存时间设置成功，返回 1 。
        /// 当 key 不存在或没办法设置生存时间时，返回 0 。 (查看: EXPIRE命令获取更多信息).
        /// http://www.redis.cn/commands/pexpireat.html
        /// http://redis.readthedocs.org/en/latest/key/pexpireat.html
        /// </summary>
        public readonly static byte[] PExpireAt = "PEXPIREAT".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// 设置key的过期时间。如果key已过期，将会被自动删除。设置了过期时间的key被称之为volatile。
        /// 在key过期之前可以重新更新他的过期时间，也可以使用PERSIST命令删除key的过期时间。
        /// 在Redis< 2.1.3之前的版本,key的生存时间可以被更新
        /// 返回值：
        /// 整数，如下的整数结果
        /// 1 如果设置了过期时间
        /// 0 如果没有设置过期时间，或者不能设置过期时间
        /// http://www.redis.cn/commands/expire.html
        /// http://redis.readthedocs.org/en/latest/key/expire.html
        /// </summary>
        public readonly static byte[] Expire = "EXPIRE".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// EXPIREAT 的作用和 EXPIRE类似，都用于为 key 设置生存时间。
        /// 不同在于 EXPIREAT 命令接受的时间参数是 UNIX 时间戳 Unix timestamp 。
        /// 返回值：
        /// 整数，如下的整数结果
        /// 1 如果设置了过期时间
        /// 0 如果没有设置过期时间，或者不能设置过期时间
        /// http://www.redis.cn/commands/expireat.html
        /// http://redis.readthedocs.org/en/latest/key/expireat.html
        /// </summary>
        public readonly static byte[] ExpireAt = "EXPIREAT".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。
        /// 返回值：
        /// 当 key 不存在时，返回 -2 。
        /// 当 key 存在但没有设置剩余生存时间时，返回 -1 。
        /// 否则，以秒为单位，返回 key 的剩余生存时间。
        /// http://www.redis.cn/commands/ttl.html
        /// http://redis.readthedocs.org/en/latest/key/ttl.html
        /// </summary>
        public readonly static byte[] Ttl = "TTL".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// 这个命令类似于 TTL 命令，但它以毫秒为单位返回 key 的剩余生存时间，而不是像 TTL 命令那样，以秒为单位。
        /// 返回值：
        /// 当 key 不存在时，返回 -2 。
        /// 当 key 存在但没有设置剩余生存时间时，返回 -1 。
        /// 否则，以毫秒为单位，返回 key 的剩余生存时间。
        /// http://www.redis.cn/commands/pttl.html
        /// http://redis.readthedocs.org/en/latest/key/pttl.html
        /// </summary>
        public readonly static byte[] PTtl = "PTTL".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： 查找给定键的复杂度为 O(1) ，对键进行序列化的复杂度为 O(N*M) ，
        /// 其中 N 是构成 key 的 Redis 对象的数量，而 M 则是这些对象的平均大小。如果序列化的对象是比较小的字符串，那么复杂度为 O(1) 。
        /// 序列化给定 key ，并返回被序列化的值，使用 RESTORE 命令可以将这个值反序列化为 Redis 键。
        /// 序列化生成的值有以下几个特点：
        /// 它带有 64 位的校验和，用于检测错误，RESTORE 在进行反序列化之前会先检查校验和。
        /// 值的编码格式和 RDB 文件保持一致。
        /// RDB 版本会被编码在序列化值当中，如果因为 Redis 的版本不同造成 RDB 格式不兼容，那么 Redis 会拒绝对这个值进行反序列化操作。
        /// 序列化的值不包括任何生存时间信息。
        /// 返回值
        /// 如果 key 不存在，那么返回 nil。
        /// 否则，返回序列化之后的值。
        /// http://www.redis.cn/commands/dump.html
        /// http://redis.readthedocs.org/en/latest/key/dump.html
        /// </summary>
        public readonly static byte[] Dump = "DUMP".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： 查找给定键的复杂度为 O(1) ，对键进行反序列化的复杂度为 O(N*M) ，
        /// 其中 N 是构成 key 的 Redis 对象的数量，而 M 则是这些对象的平均大小。
        /// 有序集合(sorted set)的反序列化复杂度为 O(N*M*log(N)) ，因为有序集合每次插入的复杂度为 O(log(N)) 。
        /// 如果反序列化的对象是比较小的字符串，那么复杂度为 O(1) 。
        ///反序列化给定的序列化值，并将它和给定的 key 关联。
        ///参数 ttl 以毫秒为单位为 key 设置生存时间；如果 ttl 为 0 ，那么不设置生存时间。
        ///RESTORE 在执行反序列化之前会先对序列化值的 RDB 版本和数据校验和进行检查，
        ///如果 RDB 版本不相同或者数据不完整的话，那么 RESTORE 会拒绝进行反序列化，并返回一个错误。
        ///返回值
        ///如果反序列化成功那么返回 OK ，否则返回一个错误。
        ///http://www.redis.cn/commands/restore.html
        ///http://redis.readthedocs.org/en/latest/key/restore.html
        /// </summary>
        public readonly static byte[] Restore = "RESTORE".ToUtf8Bytes();
        /// <summary>
        /// 原子性的将key从redis的一个实例移到另一个实例
        /// http://www.redis.cn/commands/migrate.html
        /// http://redis.readthedocs.org/en/latest/key/migrate.html
        /// </summary>
        public readonly static byte[] Migrate = "MIGRATE".ToUtf8Bytes();
        /// <summary>
        /// 时间复杂度： O(1)。
        /// 将当前数据库的 key 移动到给定的数据库 db 当中。
        /// 如果当前数据库(源数据库)和给定数据库(目标数据库)有相同名字的给定 key ，或者 key 不存在于当前数据库，那么 MOVE 没有任何效果。
        /// 因此，也可以利用这一特性，将 MOVE 当作锁(locking)原语(primitive)。
        /// 返回值：
        /// 移动成功返回 1
        /// 失败则返回 0
        /// http://www.redis.cn/commands/move.html
        /// http://redis.readthedocs.org/en/latest/key/move.html
        /// </summary>
        public readonly static byte[] Move = "MOVE".ToUtf8Bytes();
        /// <summary>
        /// DEBUG OBJECT 是一个不应该有客户端调用的调试命令，应该用OBJECT命令来代替。
        /// http://www.redis.cn/commands/debug-object.html
        /// http://redis.readthedocs.org/en/latest/key/object.html
        /// </summary>
        public readonly static byte[] Object = "OBJECT".ToUtf8Bytes();
        /// <summary>
        /// PERSIST key
        /// 移除key的过期时间
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// 移除给定key的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
        /// Return value
        /// Integer reply, specifically:
        /// 当生存时间移除成功时，返回 1 .
        /// 如果 key 不存在或 key 没有设置生存时间，返回 0 .
        /// http://www.redis.cn/commands/persist.html
        /// http://redis.readthedocs.org/en/latest/key/persist.html
        /// </summary>
        public readonly static byte[] Persist = "PERSIST".ToUtf8Bytes();
        /// <summary>
        /// SCAN cursor [MATCH pattern] [COUNT count]
        /// 增量迭代key
        /// 加入版本 2.8.0。
        /// 时间复杂度：增量式迭代命令每次执行的复杂度为O(1) ,对数据集进行一次完整迭代的复杂度为O(N) ,其中 N 为数据集中的元素数量。
        /// http://www.redis.cn/commands/scan.html
        /// http://redis.readthedocs.org/en/latest/key/scan.html
        /// </summary>
        public readonly static byte[] Scan = "SCAN".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// 对队列、集合、有序集合排序
        /// http://www.redis.cn/commands/sort.html
        /// http://redis.readthedocs.org/en/latest/key/sort.html
        /// </summary>
        public readonly static byte[] Sort = "SORT".ToUtf8Bytes();
        #endregion

        #region Server（服务器）
        /// <summary>
        /// 返回当前数据里面keys的数量。
        /// 返回值:Integer reply
        /// http://www.redis.cn/commands/dbsize.html
        /// http://redis.readthedocs.org/en/latest/server/dbsize.html
        /// </summary>
        public readonly static byte[] DbSize = "DBSIZE".ToUtf8Bytes();
        /// <summary>
        /// 删除当前数据库里面的所有数据，这个命令永远不会出现失败。
        /// 返回值：
        /// Status code reply
        /// http://www.redis.cn/commands/flushdb.html
        /// http://redis.readthedocs.org/en/latest/server/flushdb.html
        /// </summary>
        public readonly static byte[] FlushDb = "FLUSHDB".ToUtf8Bytes();
        /// <summary>
        /// 删除所有数据库里面的所有数据，注意不是当前数据库，而是所有数据库，这个命令永远不会出现失败。
        /// 返回值：
        /// Status code reply
        /// http://www.redis.cn/commands/flushall.html
        /// http://redis.readthedocs.org/en/latest/server/flushall.html
        /// </summary>
        public readonly static byte[] FlushAll = "FLUSHALL".ToUtf8Bytes();
        /// <summary>
        /// SAVE 命令执行一个同步操作，以RDB文件的方式保存所有数据的快照
        /// 很少在生产环境直接使用SAVE 命令，因为它会阻塞所有的客户端的请求，可以使用BGSAVE 命令代替. 
        /// 如果在BGSAVE命令的保存数据的子进程发生错误的时,用 SAVE命令保存最新的数据是最后的手段,详细的说明请参考持久化文档
        /// 返回值:
        /// 返回状态码: 命令成功返回OK.
        /// http://www.redis.cn/commands/save.html
        /// http://redis.readthedocs.org/en/latest/server/save.html
        /// </summary>
        public readonly static byte[] Save = "SAVE".ToUtf8Bytes();
        /// <summary>
        /// 后台保存DB。会立即返回 OK 状态码。 
        /// Redis forks, 父进程继续提供服务以供客户端调用，子进程将DB数据保存到磁盘然后退出。
        /// 如果操作成功，可以通过客户端命令LASTSAVE来检查操作结果。
        /// Please refer to the persistence documentation for detailed information.
        /// 返回值:
        /// Status code reply
        /// http://www.redis.cn/commands/bgsave.html
        /// http://redis.readthedocs.org/en/latest/server/bgsave.html
        /// </summary>
        public readonly static byte[] BgSave = "BGSAVE".ToUtf8Bytes();
        /// <summary>
        /// 执行成功时返回UNIX时间戳。. 
        /// 客户端执行 BGSAVE 命令时，可以通过每N秒发送一个 LASTSAVE 命令来查看BGSAVE 命令执行的结果，
        /// 由 LASTSAVE 返回结果的变化可以判断执行结果。
        /// 返回值
        /// Integer reply: UNIX 的时间戳.
        /// http://www.redis.cn/commands/lastsave.html
        /// http://redis.readthedocs.org/en/latest/server/lastsave.html
        /// </summary>
        public readonly static byte[] LastSave = "LASTSAVE".ToUtf8Bytes();
        /// <summary>
        /// 这个命令执行如下操作::
        ///     停止所有客户端.
        ///     如果配置了save 策略 则执行一个阻塞的save命令.
        ///     如果开启了AOF,则刷新aof文件..
        ///     关闭redis服务进程（redis-server）.
        ///     
        /// 如果配置了持久化策略，那么这个命令将能够保证在关闭redis服务进程的时候数据不会丢失. 
        /// 如果仅仅在客户端执行SAVE 命令,然后 执行QUIT 命令，那么数据的完整性将不会被保证，因为其他客户端可能在执行这两个命令的期间修改数据库的数据.
        /// 注意: 一个没有配置持久化策略的redis实例 (没有aof配置, 没有 "save" 命令) 将不会 在执行SHUTDOWN命令的时候转存一个rdb文件, 
        /// 通常情况下你不想让一个仅用于缓存的rendis实例宕掉
        /// 
        /// SAVE 和 NOSAVE 修饰符
        /// 通过指定一个可选的修饰符可以改变这个命令的表现形式 比如::
        /// SHUTDOWN SAVE   能够在即使没有配置持久化的情况下强制数据库存储.
        /// SHUTDOWN NOSAVE 能够在配置一个或者多个持久化策略的情况下阻止数据库存储. (你可以假想它为一个中断服务的 ABORT 命令).
        /// 返回值
        /// 当发生错误的时候返回状态码 . 当成功的时候不返回任何值，服务退出，链接关闭.
        /// http://www.redis.cn/commands/shutdown.html
        /// http://redis.readthedocs.org/en/latest/server/shutdown.html
        /// </summary>
        public readonly static byte[] Shutdown = "SHUTDOWN".ToUtf8Bytes();
        /// <summary>
        /// 异步重写追加文件
        /// 返回值
        /// Status code reply: always OK.
        /// http://www.redis.cn/commands/bgrewriteaof.html
        /// http://redis.readthedocs.org/en/latest/server/bgrewriteaof.html
        /// </summary>
        public readonly static byte[] BgRewriteAof = "BGREWRITEAOF".ToUtf8Bytes();
        /// <summary>
        /// 获得服务器的详细信息
        /// http://www.redis.cn/commands/info.html
        /// http://redis.readthedocs.org/en/latest/server/info.html
        /// </summary>
        public readonly static byte[] Info = "INFO".ToUtf8Bytes();
        /// <summary>
        /// 指定当前服务器的主服务器
        /// SLAVEOF 命令用于在 Redis 运行时动态地修改复制(replication)功能的行为。
        /// 通过执行 SLAVEOF host port 命令，可以将当前服务器转变为指定服务器的从属服务器(slave server)。
        /// http://www.redis.cn/commands/slaveof.html
        /// http://redis.readthedocs.org/en/latest/server/slaveof.html
        /// </summary>
        public readonly static byte[] SlaveOf = "SLAVEOF".ToUtf8Bytes();

        public readonly static byte[] No = "NO".ToUtf8Bytes();

        public readonly static byte[] One = "ONE".ToUtf8Bytes();
        /// <summary>
        /// 重置INFO命令统计里面的一些计算器。
        /// 被重置的数据如下:
        /// Keyspace hits
        /// Keyspace misses
        /// Number of commands processed
        /// Number of connections received
        /// Number of expired keys
        /// 返回值
        /// Status code reply: 总是返回 OK.
        /// http://www.redis.cn/commands/config-resetstat.html
        /// http://redis.readthedocs.org/en/latest/server/config_resetstat.html
        /// </summary>
        public readonly static byte[] ResetStat = "RESETSTAT".ToUtf8Bytes();
        /// <summary>
        /// 返回当前服务器时间
        /// TIME 命令返回当前Unix时间戳和当天已经过去的微秒的数。 基本上，该接口非常相似gettimeofday.
        /// Multi-bulk reply, specifically:
        /// 返回内容包含两个元素
        /// UNIX时间戳（单位：秒）
        /// 微秒
        /// http://www.redis.cn/commands/time.html
        /// http://redis.readthedocs.org/en/latest/server/time.html
        /// </summary>
        public readonly static byte[] Time = "TIME".ToUtf8Bytes();
        /// <summary>
        /// DEBUG SEGFAULT执行在崩溃的Redis一个无效的内存访问，它是用来模拟在开发过程中的错误。
        /// Status code reply
        /// http://www.redis.cn/commands/debug-segfault.html
        /// http://redis.readthedocs.org/en/latest/server/debug_segfault.html
        /// </summary>
        public readonly static byte[] Segfault = "SEGFAULT".ToUtf8Bytes();
        /// <summary>
        /// 
        /// </summary>
        public readonly static byte[] IdleTime = "IDLETIME".ToUtf8Bytes();

        /// <summary>
        /// 实时监控服务器
        /// MONITOR 是一个调试命令，返回服务器处理的每一个命令，它能帮助我们了解在数据库上发生了什么操作，可以通过redis-cli和telnet命令使用.
        /// 使用SIGINT (Ctrl-C)来停止 通过redis-cli使用MONITOR 命令返回的输出.
        /// 使用 QUIT命令来停止通过telnet使用MONITOR返回的输出.
        /// 
        /// MONITOR 性能消耗
        /// 由于MONITOR命令返回 服务器处理的所有的 命令, 所以在性能上会有一些消耗.
        /// 在不运行 MONITOR 命令的情况下，benchmark的测试结果:
        /// $ src/redis-benchmark -c 10 -n 100000 -q
        /// PING_INLINE: 101936.80 requests per second
        /// PING_BULK: 102880.66 requests per second
        /// SET: 95419.85 requests per second
        /// GET: 104275.29 requests per second
        /// INCR: 93283.58 requests per second
        /// 在运行 MONITOR 命令的情况下，benchmark的测试结果: (redis-cli monitor > /dev/null):
        /// $ src/redis-benchmark -c 10 -n 100000 -q
        /// PING_INLINE: 58479.53 requests per second
        /// PING_BULK: 59136.61 requests per second
        /// SET: 41823.50 requests per second
        /// GET: 45330.91 requests per second
        /// INCR: 41771.09 requests per second
        /// 在这种特定的情况下，运行一个 MONITOR 命令能够降低50%的吞吐量，运行多个MONITOR命令 降低的吞吐量更多.
        /// 返回值
        /// 没有统一标准的返回值, 无限的返回服务器端处理的命令流.
        /// 
        /// http://www.redis.cn/commands/monitor.html
        /// http://redis.readthedocs.org/en/latest/server/monitor.html
        /// </summary>
        public readonly static byte[] Monitor = "MONITOR".ToUtf8Bytes();		//missing

        /// <summary>
        /// 调试的前缀命令
        /// DEBUG OBJECT
        /// http://redis.readthedocs.org/en/latest/server/debug_object.html
        /// DEBUG SEGFAULT
        /// http://redis.readthedocs.org/en/latest/server/debug_segfault.html
        /// </summary>
        public readonly static byte[] Debug = "DEBUG".ToUtf8Bytes();			//missing

        /// <summary>
        /// 获取配置的前缀命令
        /// CONFIG GET
        /// http://redis.readthedocs.org/en/latest/server/config_get.html
        /// CONFIG RESETSTAT
        /// http://redis.readthedocs.org/en/latest/server/config_resetstat.html
        /// CONFIG REWRITE
        /// http://redis.readthedocs.org/en/latest/server/config_rewrite.html
        /// CONFIG SET
        /// http://redis.readthedocs.org/en/latest/server/config_set.html
        /// </summary>
        public readonly static byte[] Config = "CONFIG".ToUtf8Bytes();			//missing

        /// <summary>
        /// 与客户端有关命令的前缀命令
        /// CLIENT GETNAME
        /// http://redis.readthedocs.org/en/latest/server/client_getname.html
        /// CLIENT KILL
        /// http://redis.readthedocs.org/en/latest/server/client_kill.html
        /// CLIENT LIST
        /// http://redis.readthedocs.org/en/latest/server/client_list.html
        /// CLIENT SETNAME
        /// http://redis.readthedocs.org/en/latest/server/client_setname.html
        /// </summary>
        public readonly static byte[] Client = "CLIENT".ToUtf8Bytes();

        /// <summary>
        /// CLIENT LIST 
        /// 获得客户端连接列表
        /// http://www.redis.cn/commands/client-list.html
        /// http://redis.readthedocs.org/en/latest/server/client_list.html
        /// </summary>
        public readonly static byte[] List = "LIST".ToUtf8Bytes();
        
        /// <summary>
        /// CLIENT KILL ip:port
        /// 关闭客户端连接
        /// http://www.redis.cn/commands/client-kill.html
        /// http://redis.readthedocs.org/en/latest/server/client_kill.html
        /// 
        /// SCRIPT KILL 
        /// 杀死当前正在运行的 Lua 脚本。
        /// http://www.redis.cn/commands/script-kill.html
        /// http://redis.readthedocs.org/en/latest/script/script_kill.html
        /// </summary>
        public readonly static byte[] Kill = "KILL".ToUtf8Bytes();

        /// <summary>
        /// CLIENT SETNAME connection-name
        /// 设置当前连接的名字
        /// http://www.redis.cn/commands/client-setname.html
        /// http://redis.readthedocs.org/en/latest/server/client_setname.html
        /// </summary>
        public readonly static byte[] SetName = "SETNAME".ToUtf8Bytes();

        /// <summary>
        /// CLIENT GETNAME 
        /// 获得当前连接名称
        /// http://www.redis.cn/commands/client-getname.html
        /// http://redis.readthedocs.org/en/latest/server/client_getname.html
        /// </summary>
        public readonly static byte[] GetName = "GETNAME".ToUtf8Bytes();
        /// <summary>
        /// SLOWLOG subcommand [argument]
        /// 管理再分配的慢查询日志
        /// 加入版本 2.2.12。
        /// 此命令是为了读取和复位再分配的慢查询日志。
        /// http://www.redis.cn/commands/slowlog.html
        /// http://redis.readthedocs.org/en/latest/server/slowlog.html
        /// </summary>
        public readonly static byte[] Slowlog = "SLOWLOG".ToUtf8Bytes();
        #endregion

        #region String（字符串）
        /// <summary>
        /// STRLEN key
        /// 获取指定key值的长度
        /// http://www.redis.cn/commands/strlen.html
        /// http://redis.readthedocs.org/en/latest/string/strlen.html
        /// </summary>
        public readonly static byte[] StrLen = "STRLEN".ToUtf8Bytes();

        /// <summary>
        /// SET key value
        /// 设置一个key的value值
        /// 时间复杂度： O(1)。
        /// 将key和value对应。如果key已经存在了，它会被覆盖，而不管它是什么类型。
        /// 返回值
        /// 状态码：总是OK，因为SET不会失败。
        /// http://www.redis.cn/commands/set.html
        /// http://redis.readthedocs.org/en/latest/string/set.html
        /// </summary>
        public readonly static byte[] Set = "SET".ToUtf8Bytes();

        /// <summary>
        /// GET key
        /// 获取key的值
        /// 时间复杂度： O(1)。
        /// 返回key的value。如果key不存在，返回特殊值nil。如果key的value不是string，就返回错误，因为GET只处理string类型的values。
        /// 返回值
        /// Bulk reply:key对应的value，或者nil（key不存在时）
        /// http://www.redis.cn/commands/get.html
        /// http://redis.readthedocs.org/en/latest/string/get.html
        /// </summary>
        public readonly static byte[] Get = "GET".ToUtf8Bytes();

        /// <summary>
        /// GETSET key value
        /// 设置一个key的value，并获取设置前的值
        /// 时间复杂度： O(1)。
        /// 自动将key对应到value并且返回原来key对应的value。
        /// 如果key存在但是对应的value不是字符串，就返回错误。
        /// 设计模式
        /// GETSET可以和INCR一起使用实现支持重置的计数功能。
        /// 举个例子：每当有事件发生的时候，一段程序都会调用INCR给key mycounter加1，
        /// 但是有时我们需要获取计数器的值，并且自动将其重置为0。这可以通过GETSET mycounter "0"来实现：
        /// http://www.redis.cn/commands/getset.html
        /// http://redis.readthedocs.org/en/latest/string/getset.html
        /// </summary>
        public readonly static byte[] GetSet = "GETSET".ToUtf8Bytes();

        /// <summary>
        /// MGET key [key ...]
        /// 获得所有key的值
        /// 时间复杂度： O(N)，这里N是要处理的key的个数。
        /// 返回所有指定的key的value。对于每个不对应string或者不存在的key，都返回特殊值nil。
        /// 正因为此，这个操作从来不会失败。
        /// 返回值
        /// 多返回值: 指定的key对应的values的list
        /// http://www.redis.cn/commands/mget.html
        /// http://redis.readthedocs.org/en/latest/string/mget.html
        /// </summary>
        public readonly static byte[] MGet = "MGET".ToUtf8Bytes();

        /// <summary>
        /// SETNX key value
        /// 设置的一个关键的价值，只有当该键不存在
        /// 如果key不存在，就设置key对应字符串value。
        /// 在这种情况下，该命令和SET一样。当key已经存在时，就不做任何操作。
        /// SETNX是"SET if Not eXists"。
        /// 返回值
        /// 数字，只有以下两种值：
        /// 1 如果key被set
        /// 0 如果key没有被set
        /// http://www.redis.cn/commands/setnx.html
        /// http://redis.readthedocs.org/en/latest/string/setnx.html
        /// </summary>
        public readonly static byte[] SetNx = "SETNX".ToUtf8Bytes();

        /// <summary>
        /// SETEX key seconds value
        /// 设置key-value并设置过期时间（单位：秒）
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 设置key对应字符串value，并且设置key在给定的seconds时间之后超时过期。这个命令等效于执行下面的命令：
        /// SET mykey value
        /// EXPIRE mykey seconds
        /// SETEX是原子的，也可以通过把上面两个命令放到MULTI/EXEC块中执行的方式重现。相比连续执行上面两个命令，它更快，因为当Redis当做缓存使用时，这个操作更加常用。
        /// 返回值
        /// 状态码
        /// http://www.redis.cn/commands/setex.html
        /// http://redis.readthedocs.org/en/latest/string/setex.html
        /// </summary>
        public readonly static byte[] SetEx = "SETEX".ToUtf8Bytes();

        /// <summary>
        /// PSETEX key milliseconds value
        /// Set the value and expiration in milliseconds of a key
        /// 加入版本 2.6.0。
        /// 时间复杂度： O(1)。
        /// PSETEX works exactly like SETEX with the sole difference that the expire time is specified in milliseconds instead of seconds.
        /// http://www.redis.cn/commands/psetex.html
        /// http://redis.readthedocs.org/en/latest/string/psetex.html
        /// </summary>
        public readonly static byte[] PSetEx = "PSETEX".ToUtf8Bytes();

        /// <summary>
        /// MSET key value [key value ...]
        /// 设置多个key value
        /// 加入版本 1.0.1。
        /// 时间复杂度： O(N)，这里N是要set的key的个数。
        /// 对应给定的keys到他们相应的values上。MSET会用新的value替换已经存在的value，就像普通的SET命令一样。如果你不想覆盖已经存在的values，请参看命令MSETNX。
        /// MSET是原子的，所以所有给定的keys是一次性set的。客户端不可能看到这种一部分keys被更新而另外的没有改变的情况。
        /// 返回值
        /// 状态码：总是OK，因为MSET不会失败。
        /// http://www.redis.cn/commands/mset.html
        /// http://redis.readthedocs.org/en/latest/string/mset.html
        /// </summary>
        public readonly static byte[] MSet = "MSET".ToUtf8Bytes();

        /// <summary>
        /// MSETNX key value [key value ...]
        /// 设置多个key value,仅当key存在时
        /// 加入版本 1.0.1。
        /// 时间复杂度： O(N)，N是要set的keys的个数。
        /// 对应给定的keys到他们相应的values上。只要有一个key已经存在，MSETNX一个操作都不会执行。
        /// 由于这种特性，MSETNX可以实现要么所有的操作都成功，要么一个都不执行，这样可以用来设置不同的key，来表示一个唯一的对象的不同字段。
        /// MSETNX是原子的，所以所有给定的keys是一次性set的。客户端不可能看到这种一部分keys被更新而另外的没有改变的情况。
        /// 返回值
        /// 数字，只有以下两种值：
        /// 1 如果所有的key被set
        /// 0 如果没有key被set(至少其中有一个key是存在的)
        /// http://www.redis.cn/commands/msetnx.html
        /// http://redis.readthedocs.org/en/latest/string/msetnx.html
        /// </summary>
        public readonly static byte[] MSetNx = "MSETNX".ToUtf8Bytes();

        /// <summary>
        /// INCR key
        /// 执行原子加1操作
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 对key对应的数字做加1操作。如果key不存在，那么在操作之前，这个key对应的值会被置为0。
        /// 如果key有一个错误类型的value或者是一个不能表示成数字的字符串，就返回错误。
        /// 这个操作最大支持在64位有符号的整型数字。
        /// 提醒：这是一个string操作，因为Redis没有专用的数字类型。
        /// key对应的string都被解释成10进制64位有符号的整型来执行这个操作。
        /// Redis会用相应的整数表示方法存储整数，所以对于表示数字的字符串，没必要为了用字符串表示整型存储做额外开销。
        /// 返回值
        /// 整型数字：增加之后的value
        /// http://www.redis.cn/commands/incr.html
        /// http://redis.readthedocs.org/en/latest/string/incr.html
        /// </summary>
        public readonly static byte[] Incr = "INCR".ToUtf8Bytes();

        /// <summary>
        /// INCRBY key increment
        /// 执行原子增加一个整数
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 将key对应的数字加decrement。如果key不存在，操作之前，key就会被置为0。
        /// 如果key的value类型错误或者是个不能表示成数字的字符串，就返回错误。这个操作最多支持64位有符号的正型数字。
        /// 查看命令INCR了解关于增减操作的额外信息。
        /// 返回值
        /// 数字：增加之后的value值。
        /// http://www.redis.cn/commands/incrby.html
        /// http://redis.readthedocs.org/en/latest/string/incrby.html
        /// </summary>
        public readonly static byte[] IncrBy = "INCRBY".ToUtf8Bytes();

        /// <summary>
        /// INCRBYFLOAT key increment
        /// 执行原子增加一个浮点数
        /// 加入版本 2.6.0。
        /// 时间复杂度： O(1)。
        /// http://www.redis.cn/commands/incrbyfloat.html
        /// http://redis.readthedocs.org/en/latest/string/incrbyfloat.html
        /// </summary>
        public readonly static byte[] IncrByFloat = "INCRBYFLOAT".ToUtf8Bytes();

        /// <summary>
        /// DECR key
        /// 整数原子减1
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 对key对应的数字做减1操作。
        /// 如果key不存在，那么在操作之前，这个key对应的值会被置为0。
        /// 如果key有一个错误类型的value或者是一个不能表示成数字的字符串，就返回错误。
        /// 这个操作最大支持在64位有符号的整型数字。
        /// 查看命令INCR了解关于增减操作的额外信息。
        /// 返回值
        /// 数字：减小之后的value
        /// http://www.redis.cn/commands/decr.html
        /// http://redis.readthedocs.org/en/latest/string/decr.html
        /// </summary>
        public readonly static byte[] Decr = "DECR".ToUtf8Bytes();

        /// <summary>
        /// DECRBY key decrement
        /// 原子减指定的整数
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 将key对应的数字减decrement。
        /// 如果key不存在，操作之前，key就会被置为0。
        /// 如果key的value类型错误或者是个不能表示成数字的字符串，就返回错误。
        /// 这个操作最多支持64位有符号的正型数字。
        /// 查看命令INCR了解关于增减操作的额外信息。似。
        /// 返回值
        /// 返回一个数字：减少之后的value值。
        /// http://www.redis.cn/commands/decrby.html
        /// http://redis.readthedocs.org/en/latest/string/decrby.html
        /// </summary>
        public readonly static byte[] DecrBy = "DECRBY".ToUtf8Bytes();

        /// <summary>
        /// APPEND key value
        /// 追加一个值到key上
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1). 
        /// The amortized time complexity is O(1) 因为redis用的动态字符串的库在每次分配空间的时候会增加一倍的可用空闲空间，
        /// 所以在添加的value较小而且已经存在的 value是任意大小的情况下，均摊时间复杂度是O(1) 。
        /// 如果 key 已经存在，并且值为字符串，那么这个命令会把 value 追加到原来值（value）的结尾。 
        /// 如果 key 不存在，那么它将首先创建一个空字符串的key，再执行追加操作，这种情况 APPEND 将类似于 SET 操作。
        /// 返回值
        /// 整数回复（Integer reply）：返回append后字符串值（value）的长度。
        /// http://www.redis.cn/commands/append.html
        /// http://redis.readthedocs.org/en/latest/string/append.html
        /// </summary>
        public readonly static byte[] Append = "APPEND".ToUtf8Bytes();

        /// <summary>
        /// GETRANGE key start end
        /// 获取存储在key上的值的一个子字符串
        /// 加入版本 2.4.0。
        /// 时间复杂度： O(N) ，这里的N是返回的string的长度。
        /// 复杂度是由返回的字符串长度决定的，但是因为从一个已经存在的字符串创建一个子串是很容易的，所以对于较小的字符串，可以认为是O(1)的复杂度。
        /// 警告：这个命令是被改成GETRANGE的，在小于2.0的Redis版本中叫SUBSTR。 
        /// 返回key对应的字符串value的子串，这个子串是由start和end位移决定的（两者都在string内）。可
        /// 以用负的位移来表示从string尾部开始数的下标。所以-1就是最后一个字符，-2就是倒数第二个，以此类推。
        /// 这个函数处理超出范围的请求时，都把结果限制在string内。
        /// 返回值
        /// Bulk reply（专有词汇，protocol有解释）。
        /// http://www.redis.cn/commands/getrange.html
        /// http://redis.readthedocs.org/en/latest/string/getrange.html
        /// </summary>
        public readonly static byte[] GetRange = "GETRANGE".ToUtf8Bytes();

        /// <summary>
        /// SETRANGE key offset value
        /// Overwrite part of a string at key starting at the specified offset
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)，不考虑拷贝新字符串的开销。
        /// 通常这个字符串非常小，所以均摊代价为O(1)。
        /// 如果考虑的话，复杂度就是O(M)，M是参数value的长度。
        /// http://www.redis.cn/commands/setrange.html
        /// http://redis.readthedocs.org/en/latest/string/setrange.html
        /// </summary>
        public readonly static byte[] SetRange = "SETRANGE".ToUtf8Bytes();

        /// <summary>
        /// GETBIT key offset
        /// 返回位的值存储在关键的字符串值的偏移量。
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// 返回key对应的string在offset处的bit值 当offset超出了字符串长度的时候，这个字符串就被假定为由0比特填充的连续空间。
        /// 当key不存在的时候，它就认为是一个空字符串，所以offset总是超出范围，然后value也被认为是由0比特填充的连续空间。到内存分配。
        /// 返回值
        /// 整型数字：在offset处的bit值
        /// http://www.redis.cn/commands/getbit.html
        /// http://redis.readthedocs.org/en/latest/string/getbit.html
        /// </summary>
        public readonly static byte[] GetBit = "GETBIT".ToUtf8Bytes();

        /// <summary>
        /// SETBIT key offset value
        /// Sets or clears the bit at offset in the string value stored at key
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// 设置或者清空key的value(字符串)在offset处的bit值。
        /// 那个位置的bit要么被设置，要么被清空，这个由value（只能是0或者1）来决定。
        /// 当key不存在的时候，就创建一个新的字符串value。要确保这个字符串大到在offset处有bit值。
        /// 参数offset需要大于等于0，并且小于232(限制bitmap大小为512)。当key对应的字符串增大的时候，新增的部分bit值都是设置为0。
        /// 警告：当set最后一个bit(offset等于232-1)并且key还没有一个字符串value或者其value是个比较小的字符串时，
        /// Redis需要立即分配所有内存，这有可能会导致服务阻塞一会。
        /// 在一台2010MacBook Pro上，
        /// offset为232-1（分配512MB）需要～300ms，
        /// offset为230-1(分配128MB)需要～80ms，
        /// offset为228-1（分配32MB）需要～30ms，
        /// offset为226-1（分配8MB）需要8ms。
        /// 注意，一旦第一次内存分配完，后面对同一个key调用SETBIT就不会预先得到内存分配。
        /// 返回值
        /// 整型数字：在offset处原来的bit值
        /// http://www.redis.cn/commands/setbit.html
        /// http://redis.readthedocs.org/en/latest/string/setbit.html
        /// </summary>
        public readonly static byte[] SetBit = "SETBIT".ToUtf8Bytes();

        /// <summary>
        /// BITCOUNT key [start] [end]
        /// 统计字符串指定起始位置的字节数
        /// http://www.redis.cn/commands/bitcount.html
        /// http://redis.readthedocs.org/en/latest/string/bitcount.html
        /// </summary>
        public readonly static byte[] BitCount = "BITCOUNT".ToUtf8Bytes();
        #endregion 

        #region Set（集合）
        /// <summary>
        /// SSCAN key cursor [MATCH pattern] [COUNT count]
        /// 迭代set里面的元素
        /// 加入版本 2.8.0。
        /// See SCAN for SSCAN documentation.
        /// http://www.redis.cn/commands/scan.html
        /// http://redis.readthedocs.org/en/latest/set/sscan.html
        /// </summary>
        public readonly static byte[] SScan = "SSCAN".ToUtf8Bytes();
        /// <summary>
        /// SADD key member [member ...]
        /// 添加一个或者多个元素到集合(set)里
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N是被添加元素的数量.
        /// 添加一个或多个指定的member元素到集合的 key中.指定的一个或者多个元素member 
        /// 如果已经在集合key中存在则忽略.
        /// 如果集合key 不存在，则新建集合key,并添加member元素到集合key中.
        /// 如果key 的类型不是集合则返回错误.
        /// 返回值
        /// 整数返回:返回新成功添加到集合里元素的数量，不包括已经存在于集合中的元素.
        /// http://www.redis.cn/commands/sadd.html
        /// http://redis.readthedocs.org/en/latest/set/sadd.html
        /// </summary>
        public readonly static byte[] SAdd = "SADD".ToUtf8Bytes();
        /// <summary>
        /// SREM key member [member ...]
        /// 从集合里删除一个或多个key
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N是被移除元素的个数.
        /// 在key集合中移除指定的元素. 如果指定的元素不是key集合中的元素则忽略 如果key集合不存在则被视为一个空的集合，该命令返回0.
        /// 如果key的类型不是一个集合,则返回错误.
        /// 返回值
        /// 返回整数:从集合中移除元素的个数，不包括不存在的成员.
        /// http://www.redis.cn/commands/srem.html
        /// http://redis.readthedocs.org/en/latest/set/srem.html
        /// </summary>
        public readonly static byte[] SRem = "SREM".ToUtf8Bytes();
        /// <summary>
        /// SPOP key
        /// 删除并获取一个集合里面的元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 移除并返回一个集合中的随机元素
        /// 该命令与 SRANDMEMBER相似,不同的是srandmember命令返回一个随机元素但是不移除.
        /// 返回值
        /// Bulk reply:被移除的元素, 当key不存在的时候返回 nil .
        /// http://www.redis.cn/commands/spop.html
        /// http://redis.readthedocs.org/en/latest/set/spop.html
        /// </summary>
        public readonly static byte[] SPop = "SPOP".ToUtf8Bytes();
        /// <summary>
        /// SMOVE source destination member
        /// 移动集合里面的一个key到另一个集合
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 将member从source集合移动到destination集合中. 对于其他的客户端,在特定的时间元素将会作为source或者destination集合的成员出现.
        /// 如果source 集合不存在或者不包含指定的元素,这smove命令不执行任何操作并且返回0
        /// .否则对象将会从source集合中移除，并添加到destination集合中去，
        /// 如果destination集合已经存在该元素，则smove命令仅将该元素从source集合中移除.
        /// 如果source 和destination不是集合类型,则返回错误.
        /// 返回值
        /// 返回整数
        /// 如果该元素成功移除,返回1
        /// 如果该元素不是 source集合成员,无任何操作,则返回0.
        /// http://www.redis.cn/commands/smove.html
        /// http://redis.readthedocs.org/en/latest/set/smove.html
        /// </summary>
        public readonly static byte[] SMove = "SMOVE".ToUtf8Bytes();
        /// <summary>
        /// SCARD key
        /// 获取集合里面的元素数量
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 返回集合存储的key的基数 (集合元素的数量).
        /// 返回值
        /// 整数返回: 集合的基数(元素的数量),如果key不存在,则返回 0.
        /// http://www.redis.cn/commands/scard.html
        /// http://redis.readthedocs.org/en/latest/set/scard.html
        /// </summary>
        public readonly static byte[] SCard = "SCARD".ToUtf8Bytes();
        /// <summary>
        /// SISMEMBER key member
        /// 确定一个给定的值是一个集合的成员
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 返回成员 member 是否是存储的集合 key的成员.
        /// 返回值
        /// 返回整数,详细说明:
        /// 如果member元素是集合key的成员，则返回1
        /// 如果member元素不是key的成员，或者集合key不存在，则返回0
        /// http://www.redis.cn/commands/sismember.html
        /// http://redis.readthedocs.org/en/latest/set/sismember.html
        /// </summary>
        public readonly static byte[] SIsMember = "SISMEMBER".ToUtf8Bytes();
        /// <summary>
        /// SINTER key [key ...]
        /// 获得两个集合的交集
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N*M) 最坏的情况 N 为最小集合的基数,M是给定集合的个数.
        /// 返回指定所有的集合的成员的交集.
        /// 例如:
        /// key1 = {a,b,c,d}
        /// key2 = {c}
        /// key3 = {a,c,e}
        /// SINTER key1 key2 key3 = {c}
        /// 如果key不存在则被认为是一个空的集合,当给定的集合为空的时候,结果也为空.(一个集合为空，结果一直为空).
        /// 返回值
        /// Multi-bulk reply: 结果集成员的列表.
        /// http://www.redis.cn/commands/sinter.html
        /// http://redis.readthedocs.org/en/latest/set/sinter.html
        /// </summary>
        public readonly static byte[] SInter = "SINTER".ToUtf8Bytes();
        /// <summary>
        /// SINTERSTORE destination key [key ...]
        /// 获得两个集合的交集，并存储在一个关键的结果集
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N*M) N 为给定集合中基数最小的集合，M为集合的个数.
        /// 这个命令与SINTER命令类似, 但是它并不是直接返回结果集,而是将结果保存在 destination集合中.
        /// 如果destination 集合存在, 则会被重写.
        /// 返回值
        /// 返回整数: 结果集中成员的个数.
        /// http://www.redis.cn/commands/sinterstore.html
        /// http://redis.readthedocs.org/en/latest/set/sinterstore.html
        /// </summary>
        public readonly static byte[] SInterStore = "SINTERSTORE".ToUtf8Bytes();
        /// <summary>
        /// SUNION key [key ...]
        /// 添加多个set元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N为给定所有集合的总数
        /// 返回给定的多个集合的并集中的所有成员.
        /// 例如:
        /// key1 = {a,b,c,d}
        /// key2 = {c}
        /// key3 = {a,c,e}
        /// SUNION key1 key2 key3 = {a,b,c,d,e}
        /// 不存在的key可以认为是空的集合.
        /// 返回值
        /// Multi-bulk reply:并集的成员列表
        /// http://www.redis.cn/commands/sunion.html
        /// http://redis.readthedocs.org/en/latest/set/sunion.html
        /// </summary>
        public readonly static byte[] SUnion = "SUNION".ToUtf8Bytes();
        /// <summary>
        /// SUNIONSTORE destination key [key ...]
        /// 合并set元素，并将结果存入新的set里面
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N 是所有给定集合的元素总数.
        /// 该命令作用类似于SUNION命令,不同的是它并不返回结果集,而是将结果存储在destination集合中.
        /// 如果destination 已经存在,则将其覆盖.
        /// 返回值
        /// 整数返回:结果集中元素的个数.
        /// http://www.redis.cn/commands/sunionstore.html
        /// http://redis.readthedocs.org/en/latest/set/sunionstore.html
        /// </summary>
        public readonly static byte[] SUnionStore = "SUNIONSTORE".ToUtf8Bytes();
        /// <summary>
        /// SDIFF key [key ...]
        /// 获得队列不存在的元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N是给定集合中元素的个数.
        /// 返回一个集合与给定集合的差集的元素.
        /// 举例:
        /// key1 = {a,b,c,d}
        /// key2 = {c}
        /// key3 = {a,c,e}
        /// SDIFF key1 key2 key3 = {b,d}
        /// 不存在的key认为是空集.
        /// 返回值
        /// Multi-bulk reply:结果集的元素.
        /// http://www.redis.cn/commands/sdiff.html
        /// http://redis.readthedocs.org/en/latest/set/sdiff.html
        /// </summary>
        public readonly static byte[] SDiff = "SDIFF".ToUtf8Bytes();
        /// <summary>
        /// SDIFFSTORE destination key [key ...]
        /// 获得队列不存在的元素，并存储在一个关键的结果集
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N是给定集合元素的总数。
        /// 该命令类似于 SDIFF, 不同之处在于该命令不返回结果集，而是将结果存放在destination集合中.
        /// 如果destination 已经存在, 则将其覆盖重写.
        /// 返回值
        /// 返回整数: 结果集元素的个数.
        /// http://www.redis.cn/commands/sdiffstore.html
        /// http://redis.readthedocs.org/en/latest/set/sdiffstore.html
        /// </summary>
        public readonly static byte[] SDiffStore = "SDIFFSTORE".ToUtf8Bytes();
        /// <summary>
        /// SMEMBERS key
        /// 获取集合里面的所有key
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) N为集合的基数.
        /// 返回key集合所有的元素.
        /// 该命令的作用与使用一个参数的SINTER 命令作用相同.
        /// 返回值
        /// Multi-bulk reply:集合中的所有元素.
        /// http://www.redis.cn/commands/smembers.html
        /// http://redis.readthedocs.org/en/latest/set/smembers.html
        /// </summary>
        public readonly static byte[] SMembers = "SMEMBERS".ToUtf8Bytes();
        /// <summary>
        /// SRANDMEMBER key [count]
        /// 从集合里面随机获取一个key
        /// 加入版本 1.0.0。
        /// 时间复杂度： 没有count参数时间复杂度为 O(1), 否则为 O(N) 其中N是count的绝对值.
        /// 仅提供key参数,那么随机返回key集合中的一个元素.
        /// Redis 2.6开始, 可以接受 count 参数,如果count是整数且小于元素的个数，返回含有 count 个不同的元素的数组,
        /// 如果count是个整数且大于集合中元素的个数时,仅返回整个集合的所有元素,
        /// 当count是负数,则会返回一个包含count的绝对值的个数元素的数组，
        /// 如果count的绝对值大于元素的个数,则返回的结果集里会出现一个元素出现多次的情况.
        /// 仅提供key参数时,该命令作用类似于SPOP命令, 不同的是SPOP命令会将被选择的随机元素从集合中移除, 而SRANDMEMBER仅仅是返回该随记元素,而不做任何操作.
        /// 返回值
        /// Bulk reply: 不使用count 参数的情况下该命令返回随机的元素,如果key不存在则返回nil . 
        /// Multi-bulk reply: 使用count参数,则返回一个随机的元素数组,如果key不存在则返回一个空的数组.
        /// http://www.redis.cn/commands/srandmember.html
        /// http://redis.readthedocs.org/en/latest/set/srandmember.html
        /// </summary>
        public readonly static byte[] SRandMember = "SRANDMEMBER".ToUtf8Bytes();
        #endregion 

        #region Hash（哈希表）
        /// <summary>
        /// HSCAN key cursor [MATCH pattern] [COUNT count]
        /// 迭代hash里面的元素
        /// 加入版本 2.8.0。
        /// See SCAN for SSCAN documentation.
        /// http://www.redis.cn/commands/scan.html
        /// http://redis.readthedocs.org/en/latest/hash/hscan.html
        /// </summary>
        public readonly static byte[] HScan = "HSCAN".ToUtf8Bytes();
        /// <summary>
        /// HSET key field value
        /// 设置hash里面一个字段的值
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 设置 key 指定的哈希集中指定字段的值。如果 key 指定的哈希集不存在，会创建一个新的哈希集并与 key 关联。如果字段在哈希集中存在，它将被重写。
        /// 返回值
        /// 整数：含义如下
        /// 1如果field是一个新的字段
        /// 0如果field原来在map里面已经存在
        /// http://www.redis.cn/commands/hset.html
        /// http://redis.readthedocs.org/en/latest/hash/hset.html
        /// </summary>
        public readonly static byte[] HSet = "HSET".ToUtf8Bytes();
        /// <summary>
        /// HSETNX key field value
        /// 设置hash的一个字段，只有当这个字段不存在时有效
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 只在 key 指定的哈希集中不存在指定的字段时，设置字段的值。如果 key 指定的哈希集不存在，会创建一个新的哈希集并与 key 关联。如果字段已存在，该操作无效果。
        /// 返回值
        /// 整数：含义如下
        /// 1：如果字段是个新的字段，并成功赋值
        /// 0：如果哈希集中已存在该字段，没有操作被执行
        /// http://www.redis.cn/commands/hsetnx.html
        /// http://redis.readthedocs.org/en/latest/hash/hsetnx.html
        /// </summary>
        public readonly static byte[] HSetNx = "HSETNX".ToUtf8Bytes();
        /// <summary>
        /// HGET key field
        /// 读取哈希域的的值
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 返回 key 指定的哈希集中该字段所关联的值
        /// 返回值
        /// 散值：该字段所关联的值。当字段不存在或者 key 不存在时返回nil。
        /// http://www.redis.cn/commands/hget.html
        /// http://redis.readthedocs.org/en/latest/hash/hget.html
        /// </summary>
        public readonly static byte[] HGet = "HGET".ToUtf8Bytes();
        /// <summary>
        /// HMSET key field value [field value ...]
        /// 设置hash字段值
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N), 其中 N 是被设置的字段的数量。
        /// 设置 key 指定的哈希集中指定字段的值。该命令将重写所有在哈希集中存在的字段。如果 key 指定的哈希集不存在，会创建一个新的哈希集并与 key 关联
        /// 返回值
        /// 状态码
        /// http://www.redis.cn/commands/hmset.html
        /// http://redis.readthedocs.org/en/latest/hash/hmset.html
        /// </summary>
        public readonly static byte[] HMSet = "HMSET".ToUtf8Bytes();
        /// <summary>
        /// HMGET key field [field ...]
        /// 获取hash里面指定字段的值
        /// 入版本 2.0.0。
        /// 时间复杂度： O(N), 其中 N 是被请求的字段的数量。
        /// 返回 key 指定的哈希集中指定字段的值。
        /// 对于哈希集中不存在的每个字段，返回 nil 值。因为不存在的keys被认为是一个空的哈希集，对一个不存在的 key 执行 HMGET 将返回一个只含有 nil 值的列表
        /// 返回值
        /// 多个返回值：含有给定字段及其值的列表，并保持与请求相同的顺序。
        /// http://www.redis.cn/commands/hmget.html
        /// http://redis.readthedocs.org/en/latest/hash/hmget.html
        /// </summary>
        public readonly static byte[] HMGet = "HMGET".ToUtf8Bytes();
        /// <summary>
        /// HINCRBY key field increment
        /// 将哈希集中指定域的值增加给定的数字
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 增加 key 指定的哈希集中指定字段的数值。如果 key 不存在，会创建一个新的哈希集并与 key 关联。如果字段不存在，则字段的值在该操作执行前被设置为 0
        /// HINCRBY 支持的值的范围限定在 64位 有符号整数
        /// 返回值
        /// 整数：增值操作执行后的该字段的值。
        /// http://www.redis.cn/commands/hincrby.html
        /// http://redis.readthedocs.org/en/latest/hash/hincrby.html
        /// </summary>
        public readonly static byte[] HIncrBy = "HINCRBY".ToUtf8Bytes();
        /// <summary>
        /// HINCRBYFLOAT key field increment
        /// 将哈希集中指定域的值增加给定的浮点数
        /// 加入版本 2.6.0。
        /// 时间复杂度： O(1)。
        /// http://www.redis.cn/commands/hincrbyfloat.html
        /// http://redis.readthedocs.org/en/latest/hash/hincrbyfloat.html
        /// </summary>
        public readonly static byte[] HIncrByFloat = "HINCRBYFLOAT".ToUtf8Bytes();
        /// <summary>
        /// HEXISTS key field
        /// 判断给定域是否存在于哈希集中
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 返回字段是否是 key 指定的哈希集中存在的字段。
        /// 返回值
        /// 整数, 含义如下：
        /// 1 哈希集中含有该字段。
        /// 0 哈希集中不含有该存在字段，或者key不存在。
        /// http://www.redis.cn/commands/hexists.html
        /// http://redis.readthedocs.org/en/latest/hash/hexists.html
        /// </summary>
        public readonly static byte[] HExists = "HEXISTS".ToUtf8Bytes();
        /// <summary>
        /// HDEL key field [field ...]
        /// 删除一个或多个哈希域
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N), 其中 N 是要被移除的域的数量。
        /// 从 key 指定的哈希集中移除指定的域。在哈希集中不存在的域将被忽略。如果 key 指定的哈希集不存在，它将被认为是一个空的哈希集，该命令将返回0。
        /// 返回值
        /// 整数：返回从哈希集中成功移除的域的数量，不包括指出但不存在的那些域
        /// 历史
        /// 在 2.4及以上版本中 ：可接受多个域作为参数。小于 2.4版本 的 Redis 每次调用只能移除一个域
        /// 要在早期版本中以原子方式从哈希集中移除多个域，可用 MULTI/EXEC块。
        /// http://www.redis.cn/commands/hdel.html
        /// http://redis.readthedocs.org/en/latest/hash/hdel.html
        /// </summary>
        public readonly static byte[] HDel = "HDEL".ToUtf8Bytes();
        /// <summary>
        /// HLEN key
        /// 获取hash里所有字段的数量
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// 返回 key 指定的哈希集包含的字段的数量。
        /// 返回值
        /// 整数：哈希集中字段的数量，当 key 指定的哈希集不存在时返回 0
        /// http://www.redis.cn/commands/hlen.html
        /// http://redis.readthedocs.org/en/latest/hash/hlen.html
        /// </summary>
        public readonly static byte[] HLen = "HLEN".ToUtf8Bytes();
        /// <summary>
        /// HKEYS key
        /// 获取hash的所有字段
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N), 其中 N 是哈希集的大小。
        /// 返回 key 指定的哈希集中所有字段的名字。
        /// 返回值
        /// 多个返回值：哈希集中的字段列表，当 key 指定的哈希集不存在时返回空列表。
        /// http://www.redis.cn/commands/hkeys.html
        /// http://redis.readthedocs.org/en/latest/hash/hkeys.html
        /// </summary>
        public readonly static byte[] HKeys = "HKEYS".ToUtf8Bytes();
        /// <summary>
        /// HVALS key
        /// 获得hash的所有值
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N), 其中 N 是哈希集的大小。
        /// 返回 key 指定的哈希集中所有字段的值。
        /// 返回值
        /// 多个返回值：哈希集中的值的列表，当 key 指定的哈希集不存在时返回空列表。
        /// http://www.redis.cn/commands/hvals.html
        /// http://redis.readthedocs.org/en/latest/hash/hvals.html
        /// </summary>
        public readonly static byte[] HVals = "HVALS".ToUtf8Bytes();
        /// <summary>
        /// HGETALL key
        /// 从哈希集中读取全部的域和值
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N), 其中 N 是哈希集的大小。
        /// 返回 key 指定的哈希集中所有的字段和值。返回值中，每个字段名的下一个是它的值，所以返回值的长度是哈希集大小的两倍
        /// 返回值
        /// 多个返回值：哈希集中字段和值的列表。当 key 指定的哈希集不存在时返回空列表。
        /// http://www.redis.cn/commands/hgetall.html
        /// http://redis.readthedocs.org/en/latest/hash/hgetall.html
        /// </summary>
        public readonly static byte[] HGetAll = "HGETALL".ToUtf8Bytes();
        #endregion

        #region SortedSet（有序集合）
        /// <summary>
        /// ZADD key score member [score member ...]
        /// 添加到有序set的一个或多个成员，或更新的分数，如果它已经存在
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(log(N)),N是有序集合中元素的个数。
        /// 该命令添加指定的成员到key对应的有序集合中，每个成员都有一个分数。
        /// 你可以指定多个分数/成员组合。
        /// 如果一个指定的成员已经在对应的有序集合中了，那么其分数就会被更新成最新的，并且该成员会重新调整到正确的位置，以确保集合有序。
        /// 如果key不存在，就会创建一个含有这些成员的有序集合，就好像往一个空的集合中添加一样。
        /// 如果key存在，但是它并不是一个有序集合，那么就返回一个错误。
        /// 分数的值必须是一个表示数字的字符串，并且可以是double类型的浮点数。
        /// 对于有序集合的介绍，可以参考sorted sets页面。
        /// 返回值
        /// 整数, 如下整数:
        /// 返回添加到有序集合中元素的个数，不包括那种已经存在只是更新分数的元素。
        /// http://www.redis.cn/commands/zadd.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zadd.html
        /// </summary>
        public readonly static byte[] ZAdd = "ZADD".ToUtf8Bytes();
        /// <summary>
        /// ZREM key member [member ...]
        /// 从排序的集合中删除一个或多个成员
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(log(N))，N是有序集合中元素个数。 从key对应的有序集合中删除给定的成员。如果给定的成员不存在就忽略。
        /// 当key存在，但是其不是有序集合类型，就返回一个错误。
        /// 返回值
        /// 整数, 如下的整数:
        /// 返回的是从有序集合中删除的成员个数，不包括不存在的成员。
        /// http://www.redis.cn/commands/zrem.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrem.html
        /// </summary>
        public readonly static byte[] ZRem = "ZREM".ToUtf8Bytes();
        /// <summary>
        /// ZINCRBY key increment member
        /// 增量的一名成员在排序设置的评分
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(log(N))，N是有序集中元素个数。
        /// 为有序集key的成员member的score值加上增量increment。
        /// 如果key中不存在member，就在key中添加一个member，score是increment（就好像它之前的score是0.0）。
        /// 如果key不存在，就创建一个只含有指定member成员的有序集合。
        /// 当key不是有序集类型时，返回一个错误。
        /// score值必须是字符串表示的整数值或双精度浮点数，并且能接受double精度的浮点数。
        /// 也有可能给一个负数来减少score的值。
        /// 返回值
        /// Bulk reply: member成员的新score值，以字符串形式表示。
        /// http://www.redis.cn/commands/zincrby.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zincrby.html
        /// </summary>
        public readonly static byte[] ZIncrBy = "ZINCRBY".ToUtf8Bytes();
        /// <summary>
        /// ZRANK key member
        /// 确定在排序集合成员的索引
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(log(N))
        /// 返回有序集key中成员member的排名。其中有序集成员按score值递增(从小到大)顺序排列。排名以0为底，也就是说，score值最小的成员排名为0。
        /// 使用ZREVRANK命令可以获得成员按score值递减(从大到小)排列的排名。
        /// 返回值
        /// 如果member是有序集key的成员，返回member的排名的整数。
        /// 如果member不是有序集key的成员，返回Bulk reply: nil。
        /// http://www.redis.cn/commands/zrank.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrank.html
        /// </summary>
        public readonly static byte[] ZRank = "ZRANK".ToUtf8Bytes();
        /// <summary>
        /// ZREVRANK key member
        /// 确定指数在排序集的成员，下令从分数高到低
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(log(N))。
        /// 返回有序集key中成员member的排名，其中有序集成员按score值从大到小排列。排名以0为底，也就是说，score值最大的成员排名为0。
        /// 使用ZRANK命令可以获得成员按score值递增(从小到大)排列的排名。
        /// 返回值
        /// 如果member是有序集key的成员，返回member的排名。整型数字。
        /// 如果member不是有序集key的成员，返回Bulk reply: nil.
        /// http://www.redis.cn/commands/zrevrank.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrevrank.html
        /// </summary>
        public readonly static byte[] ZRevRank = "ZREVRANK".ToUtf8Bytes();
        /// <summary>
        /// ZRANGE key start stop [WITHSCORES]
        /// 返回的成员在排序设置的范围，由指数
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(log(N)+M)，N为有序集的基数，而M为结果集的基数。
        /// 返回有序集key中，指定区间内的成员。其中成员按score值递增(从小到大)来排序。具有相同score值的成员按字典序来排列。
        /// 如果你需要成员按score值递减(score相等时按字典序递减)来排列，请使用ZREVRANGE命令
        /// 下标参数start和stop都以0为底，也就是说，以0表示有序集第一个成员，以1表示有序集第二个成员，以此类推。 
        /// 你也可以使用负数下标，以-1表示最后一个成员，-2表示倒数第二个成员，以此类推。
        /// 超出范围的下标并不会引起错误。如果start的值比有序集的最大下标还要大，或是start > stop时，ZRANGE命令只是简单地返回一个空列表。
        /// 另一方面，假如stop参数的值比有序集的最大下标还要大，那么Redis将stop当作最大下标来处理。
        /// 可以通过使用WITHSCORES选项，来让成员和它的score值一并返回，返回列表以value1,score1, ..., valueN,scoreN的格式表示，而不是value1,...,valueN。
        /// 客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
        /// 返回值
        /// Multi-bulk reply: 指定范围的元素列表(可选是否含有分数)。
        /// http://www.redis.cn/commands/zrange.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrange.html
        /// </summary>
        public readonly static byte[] ZRange = "ZRANGE".ToUtf8Bytes();
        /// <summary>
        /// ZREVRANGE key start stop [WITHSCORES]
        /// 在排序的设置返回的成员范围，通过索引，下令从分数高到低
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(log(N)+M)，N为有序集的基数，而M为结果集的基数。
        /// 返回有序集key中，指定区间内的成员。其中成员的位置按score值递减(从大到小)来排列。具有相同score值的成员按字典序的反序排列。 除了成员按score值递减的次序排列这一点外，ZREVRANGE命令的其他方面和ZRANGE命令一样。
        /// 返回值
        /// Multi-bulk reply: 指定范围的元素列表(可选是否含有分数)。
        /// http://www.redis.cn/commands/zrevrange.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrevrange.html
        /// </summary>
        public readonly static byte[] ZRevRange = "ZREVRANGE".ToUtf8Bytes();
        /// <summary>
        /// ZRANGEBYSCORE key min max [WITHSCORES] [LIMIT offset count]
        /// 加入版本 1.0.5。
        /// 时间复杂度： O(log(N)+M)，N是有序集合中元素个数，M是返回的结果集中元素个数。
        /// 如果M是常量（比如，用limit总是请求前10个元素），你可以认为是O(log(N))。 
        /// 返回key的有序集合中的分数在min和max之间的所有元素（包括分数等于max或者min的元素）。
        /// 元素被认为是从低分到高分排序的。 
        /// 具有相同分数的元素按字典序排列（这个根据redis对有序集合实现的情况而定，并不需要进一步计算）。
        /// 可选的LIMIT参数指定返回结果的数量及区间（类似SQL中SELECT LIMIT offset, count）。
        /// 注意，如果offset太大，定位offset就可能遍历整个有序集合，这会增加O(N)的复杂度。 
        /// 可选参数WITHSCORES会返回元素和其分数，而不只是元素。这个选项在redis2.0之后的版本都可用。
        /// 区间及无限
        /// min和max可以是-inf和+inf，这样一来，你就可以在不知道有序集的最低和最高score值的情况下，使用ZRANGEBYSCORE这类命令。
        /// 默认情况下，区间的取值使用闭区间(小于等于或大于等于)，你也可以通过给参数前增加(符号来使用可选的******间(小于或大于)。
        /// 
        /// 举个例子：
        /// ZRANGEBYSCORE zset (1 5
        /// 返回所有符合条件1 < score <= 5的成员；
        /// ZRANGEBYSCORE zset (5 (10
        /// 返回所有符合条件5 < score < 10 的成员。
        /// 返回值
        /// Multi-bulk reply: 指定分数范围的元素列表(也可以返回他们的分数)。
        /// http://www.redis.cn/commands.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrangebyscore.html
        /// </summary>
        public readonly static byte[] ZRangeByScore = "ZRANGEBYSCORE".ToUtf8Bytes();
        /// <summary>
        /// ZREVRANGEBYSCORE key max min [WITHSCORES] [LIMIT offset count]
        /// 返回的成员在排序设置的范围，由得分，下令从分数高到低
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(log(N)+M) N是有序集合中元素个数，M是返回的结果集中元素如果M是常量（比如，用LIMIT总是请求前10个元素），你可以认为是O(log(N))。
        /// 返回key的有序集合中的分数在max和min之间的所有元素（包括分数等于max或者min的元素）。
        /// 与有序集合的默认排序相反，对于这个命令，元素被认为是从高分到低具有相同分数的元素按字典反序。
        /// 除了反序之外， ng, ZREVRANGEBYSCORE 和ZRANGEBYSCORE类似。
        /// 返回值
        /// Multi-bulk reply: 指定分数范围的元素列表(也可以返回他们的分数)。
        /// http://www.redis.cn/commands/zrevrangebyscore.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrevrangebyscore.html
        /// </summary>
        public readonly static byte[] ZRevRangeByScore = "ZREVRANGEBYSCORE".ToUtf8Bytes();
        /// <summary>
        /// ZCARD key
        /// 获取一个排序的集合中的成员数量
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(1)。
        /// 返回key的有序集元素个数。
        /// 返回值
        /// 整数: key存在的时候，返回有序集的元素个数，否则返回0。
        /// http://www.redis.cn/commands/zcard.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zcard.html
        /// </summary>
        public readonly static byte[] ZCard = "ZCARD".ToUtf8Bytes();
        /// <summary>
        /// ZSCORE key member
        /// 获取成员在排序设置相关的比分
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(1)。
        /// 返回有序集key中，成员member的score值。
        /// 如果member元素不是有序集key的成员，或key不存在，返回nil。
        /// 返回值
        /// Bulk reply: member成员的score值（double型浮点数），以字符串形式表示。
        /// http://www.redis.cn/commands/zscore.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zscore.html
        /// </summary>
        public readonly static byte[] ZScore = "ZSCORE".ToUtf8Bytes();
        /// <summary>
        /// ZCOUNT key min max
        /// 给定值范围内的成员数与分数排序
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(log(N)+M),N为有序集的基数，M为值在min和max之间的元素的数量。
        /// 返回有序集key中，score值在min和max之间(默认包括score值等于min或max)的成员。 
        /// 关于参数min和max的详细使用方法，请参考ZRANGEBYSCORE命令。
        /// 返回值
        /// 整数: 指定分数范围的元素个数。
        /// http://www.redis.cn/commands/zcount.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zcount.html
        /// </summary>
        public readonly static byte[] ZCount = "ZCOUNT".ToUtf8Bytes();
        /// <summary>
        /// ZREMRANGEBYRANK key start stop
        /// 在排序设置的所有成员在给定的索引中删除
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(log(N)+M)，N为有序集的基数，而M为被移除成员的数量。
        /// 移除有序集key中，指定排名(rank)区间内的所有成员。
        /// 下标参数start和stop都以0为底，0处是分数最小的那个元素。
        /// 这些索引也可是负数，表示位移从最高分处开始数。
        /// 例如，-1是分数最高的元素，-2是分数第二高的，依次类推。
        /// 返回值
        /// 整数: 被移除成员的数量。
        /// http://www.redis.cn/commands/zremrangebyrank.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zremrangebyrank.html
        /// </summary>
        public readonly static byte[] ZRemRangeByRank = "ZREMRANGEBYRANK".ToUtf8Bytes();
        /// <summary>
        /// ZREMRANGEBYSCORE key min max
        /// 删除一个排序的设置在给定的分数所有成员
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(log(N)+M)，N为有序集的基数，而M为被移除成员的数量。
        /// 移除有序集key中，所有score值介于min和max之间(包括等于min或max)的成员。
        /// 自版本2.1.6开始，score值等于min或max的成员也可以不包括在内，语法请参见ZRANGEBYSCORE命令。
        /// 返回值
        /// 整数: 删除的元素的个数。
        /// http://www.redis.cn/commands/zremrangebyscore.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zremrangebyscore.html
        /// </summary>
        public readonly static byte[] ZRemRangeByScore = "ZREMRANGEBYSCORE".ToUtf8Bytes();
        /// <summary>
        /// ZUNIONSTORE destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX]
        /// 添加多个排序集和导致排序的设置存储在一个新的关键
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N)+O(M log(M))，N为给定有序集基数的总和，M为结果集的基数。
        /// 计算给定的numkeys个有序集合的并集，并且把结果放到destination中。
        /// 在给定要计算的key和其它参数之前，必须先给定key个数(numberkeys)。 
        /// 默认情况下，结果集中某个成员的score值是所有给定集下该成员score值之和。
        /// 使用WEIGHTS选项，你可以为每个给定的有序集指定一个乘法因子，意思就是，每个给定有序集的所有成员的score值在传递给聚合函数之前都要先乘以该因子。
        /// 如果WEIGHTS没有给定，默认就是1。
        /// 使用AGGREGATE选项，你可以指定并集的结果集的聚合方式。默认使用的参数SUM，可以将所有集合中某个成员的score值之和作为结果集中该成员的score值。
        /// 如果使用参数MIN或者MAX，结果集就是所有集合中元素最小或最大的元素。
        /// 返回值
        /// 整数: 结果有序集合destination中元素个数。
        /// http://www.redis.cn/commands/zunionstore.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zunionstore.html
        /// </summary>
        public readonly static byte[] ZUnionStore = "ZUNIONSTORE".ToUtf8Bytes();
        /// <summary>
        /// ZINTERSTORE destination numkeys key [key ...] [WEIGHTS weight [weight ...]] [AGGREGATE SUM|MIN|MAX]
        /// 相交多个排序集，导致排序的设置存储在一个新的关键
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N*K)+O(M*log(M))最差情况下N是输入的有序集合中最小的，K是有序集合个数，并且M是结果集中元素的个数。
        /// 计算给定的numkeys个有序集合的交集，并且把结果放到destination中。
        /// 在给定要计算的key和其它参数之前，必须先给定key个数(numberkeys)。
        /// 默认情况下，结果中一个元素的分数是有序集合中该元素分数之和，前提是该元素在这些有序集合中都存在。
        /// 因为交集要求其成员必须是给定的每个有序集合中的成员，结果集中的每个元素的分数和输入的有序集合个数相等。
        /// 对于WEIGHTS 和AGGREGATE参数的描述，参见命令ZUNIONSTORE。
        /// 如果destination存在，就把它覆盖。
        /// 返回值
        /// 整数: 结果有序集合destination中元素个数。
        /// http://www.redis.cn/commands/zinterstore.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zinterstore.html
        /// </summary>
        public readonly static byte[] ZInterStore = "ZINTERSTORE".ToUtf8Bytes();
        /// <summary>
        /// ZRANGEBYLEX key min max [LIMIT offset count]
        /// 命令返回存储在键的排序集合在指定的范围元素。该元素被认为是从最低到最高的分值进行排序。
        /// 字典顺序用于以相等的分数的元素。
        /// 两个开始和结束是从零开始的索引，其中0是第一个元素，1是下一个元素等等。
        /// 它们也可以是表示偏移量从有序集的结尾，以-1作为排序的集合的最后一个元素，-2倒数第二元素等负数。
        /// 返回值
        /// 返回数组，在规定的分数范围内的元素列表。
        /// 例子
        /// redis 127.0.0.1:6379> ZADD myzset 0 a 0 b 0 c 0 d 0 e
        /// (integer) 5
        /// redis 127.0.0.1:6379> ZADD myzset 0 f 0 g
        /// (integer) 2
        /// redis 127.0.0.1:6379> ZRANGEBYLEX myzset - [c
        /// 1) "a"
        /// 2) "b"
        /// 3) "c"
        /// redis 127.0.0.1:6379> ZRANGEBYLEX myzset - (c
        /// 1) "a"
        /// 2) "b"
        /// http://www.yiibai.com/redis/sorted_sets_zrangebylex.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zrangebylex.html
        /// </summary>
        public static readonly byte[] ZRangeByLex = "ZRANGEBYLEX".ToUtf8Bytes();
        /// <summary>
        /// ZLEXCOUNT key min max 
        /// 计算一个给定的字典范围之间的有序集合成员的数量
        /// 当在一个有序集合所有元素以相同分数插入，以迫使按字典排序，此命令将返回在与最小值和最大值之间的值，在键的有序集合的元素个数。
        /// 返回值
        /// 返回整型，在指定的得分范围内的元素的数量。
        /// 
        /// 例子
        /// redis 127.0.0.1:6379> ZADD myzset 0 a 0 b 0 c 0 d 0 e
        /// (integer) 5
        /// redis 127.0.0.1:6379> ZADD myzset 0 f 0 g
        /// (integer) 2
        /// redis 127.0.0.1:6379> ZLEXCOUNT myzset - +
        /// (integer) 7
        /// redis 127.0.0.1:6379> ZLEXCOUNT myzset [b [f
        /// (integer) 5
        /// http://www.yiibai.com/redis/sorted_sets_zlexcount.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zlexcount.html
        /// </summary>
        public static readonly byte[] ZLexCount = "ZLEXCOUNT".ToUtf8Bytes();
        /// <summary>
        /// ZREMRANGEBYLEX key min max 
        /// 删除所有成员在给定的字典范围之间的有序集合
        /// 删除有序集合存储在由最小和最大指定的字典范围之间的所有键元素。
        /// 返回值
        /// 返回整数，删除的元素数量。
        /// 例子
        /// redis 127.0.0.1:6379> ZADD myzset 0 a 1 b 2 c 3 d 4 e
        /// (integer) 5
        /// redis 127.0.0.1:6379> ZREMRANGEBYLEX myzset 0 -1
        /// (integer) 1
        /// redis 127.0.0.1:6379> ZRANGE myzset 0 -1 WITHSCORES
        /// http://redis.readthedocs.org/en/latest/sorted_set/zremrangebylex.html
        /// </summary>
        public static readonly byte[] ZRemRangeByLex = "ZREMRANGEBYLEX".ToUtf8Bytes();

        /// <summary>
        /// ZSCAN key cursor [MATCH pattern] [COUNT count]
        /// 迭代sorted sets里面的元素
        /// 加入版本 2.8.0。
        /// See SCAN for SSCAN documentation.
        /// http://www.redis.cn/commands/scan.html
        /// http://redis.readthedocs.org/en/latest/sorted_set/zscan.html
        /// </summary>
        public readonly static byte[] ZScan = "ZSCAN".ToUtf8Bytes();
        /// <summary>
        /// SCAN的MATCH 选项
        /// http://www.redis.cn/commands/scan.html
        /// http://redis.readthedocs.org/en/latest/key/scan.html#scan
        /// </summary>
        public readonly static byte[] Match = "MATCH".ToUtf8Bytes();
        /// <summary>
        /// SCAN的COUNT 选项
        /// http://www.redis.cn/commands/scan.html
        /// http://redis.readthedocs.org/en/latest/key/scan.html#scan
        /// </summary>
        public readonly static byte[] Count = "COUNT".ToUtf8Bytes();
        #endregion

        #region HyperLogLog

        /// <summary>
        /// PFADD key element [element ...]
        /// Adds the specified elements to the specified HyperLogLog.
        /// 加入版本 2.8.9。
        /// 时间复杂度：每添加一个元素的复杂度为O(1).
        /// 将除了第一个参数以外的参数存储到以第一个参数为变量名的HyperLogLog结构中.
        /// 返回值
        /// 指定的整数
        /// 如果 HyperLogLog 的内部被修改了,那么返回 1,否则返回 0 .
        /// http://www.redis.cn/commands/pfadd.html
        /// http://redis.readthedocs.org/en/latest/hyperloglog/pfadd.html
        /// </summary>
        public readonly static byte[] PfAdd = "PFADD".ToUtf8Bytes();        
        /// <summary>
        /// PFCOUNT key [key ...]
        /// Return the approximated cardinality of the set(s) observed by the HyperLogLog at key(s).
        /// 加入版本 2.8.9。
        /// 时间复杂度： 使用一个key为参数时，时间复杂度为O(1)，是一个非常短的常数时间，当参数为多个key时，
        /// 时间复杂度为O(N),N为给定keys的个数，同时这个常数时间值也会比单个key所用的时间长很多.
        /// http://www.redis.cn/commands/pfcount.html
        /// http://redis.readthedocs.org/en/latest/hyperloglog/pfcount.html
        /// </summary>
        public readonly static byte[] PfCount = "PFCOUNT".ToUtf8Bytes();
        /// <summary>
        /// PFMERGE destkey sourcekey [sourcekey ...]
        /// Merge N different HyperLogLogs into a single one.
        /// 加入版本 2.8.9。
        /// 时间复杂度： O(N) 其中 N 为被合并的 HyperLogLog 数量, 这个不过程时间会比较长.
        /// 将多个 HyperLogLog 合并（merge）为一个 HyperLogLog ， 合并后的 HyperLogLog 的基数接近于所有输入 HyperLogLog 的可见集合（observed set）的并集.
        /// 合并得出的 HyperLogLog 会被储存在目标变量（第一个参数）里面， 如果该键并不存在， 那么命令在执行之前， 会先为该键创建一个空的.
        /// 返回值
        /// 简单字符串返回: 这个命令只会返回 OK.
        /// 例子
        ///redis> PFADD hll1 foo bar zap a
        ///(integer) 1
        ///redis> PFADD hll2 a b c foo
        ///(integer) 1
        ///redis> PFMERGE hll3 hll1 hll2
        ///OK
        ///redis> PFCOUNT hll3
        ///(integer) 6
        ///redis> 
        /// http://www.redis.cn/commands/pfmerge.html
        /// http://redis.readthedocs.org/en/latest/hyperloglog/pfmerge.html
        /// </summary>
        public readonly static byte[] PfMerge = "PFMERGE".ToUtf8Bytes();
        #endregion

        #region List（列表）
        /// <summary>
        /// RPUSH key value [value ...]
        /// 从队列的右边入队一个元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 向存于 key 的列表的尾部插入所有指定的值。
        /// 如果 key 不存在，那么会创建一个空的列表然后再进行 push 操作。 
        /// 当 key 保存的不是一个列表，那么会返回一个错误。
        /// 可以使用一个命令把多个元素打入队列，只需要在命令后面指定多个参数。
        /// 元素是从左到右一个接一个从列表尾部插入。 
        /// 比如命令 RPUSH mylist a b c 会返回一个列表，其第一个元素是 a ，第二个元素是 b ，第三个元素是 c。
        /// 返回值
        /// 整型回复: 在 push 操作后的列表长度。
        /// http://www.redis.cn/commands/rpush.html
        /// http://redis.readthedocs.org/en/latest/list/rpush.html
        /// </summary>
        public readonly static byte[] RPush = "RPUSH".ToUtf8Bytes();
        /// <summary>
        /// LPUSH key value [value ...]
        /// 从队列的左边入队一个或多个元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 将所有指定的值插入到存于 key 的列表的头部。
        /// 如果 key 不存在，那么在进行 push 操作前会创建一个空列表。
        /// 如果 key 对应的值不是一个 list 的话，那么会返回一个错误。
        /// 可以使用一个命令把多个元素 push 进入列表，只需在命令末尾加上多个指定的参数。
        /// 元素是从最左端的到最右端的、一个接一个被插入到 list 的头部。
        /// 所以对于这个命令例子 LPUSH mylist a b c，返回的列表是 c 为第一个元素， b 为第二个元素， a 为第三个元素。
        /// 返回值
        /// 整型回复: 在 push 操作后的 list 长度。
        /// http://www.redis.cn/commands/lpush.html
        /// http://redis.readthedocs.org/en/latest/list/lpush.html
        /// </summary>
        public readonly static byte[] LPush = "LPUSH".ToUtf8Bytes();
        /// <summary>
        /// RPUSHX key value
        /// 从队列的右边入队一个元素，仅队列存在时有效
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// 将值 value 插入到列表 key 的表尾, 当且仅当 key 存在并且是一个列表。 和 RPUSH 命令相反, 当 key 不存在时，RPUSHX 命令什么也不做。
        /// 返回值
        /// Integer: RPUSHX 命令执行之后，表的长度。
        /// http://www.redis.cn/commands/rpushx.html
        /// http://redis.readthedocs.org/en/latest/list/rpushx.html
        /// </summary>
        public readonly static byte[] RPushX = "RPUSHX".ToUtf8Bytes();
        /// <summary>
        /// LPUSHX key value
        /// 当队列存在时，从队到左边入队一个元素
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// 只有当 key 已经存在并且存着一个 list 的时候，在这个 key 下面的 list 的头部插入 value。 与 LPUSH 相反，当 key 不存在的时候不会进行任何操作。
        /// 返回值
        /// 整型回复: 在 push 操作后的 list 长度。
        /// http://www.redis.cn/commands/lpushx.html
        /// http://redis.readthedocs.org/en/latest/list/lpushx.html
        /// </summary>
        public readonly static byte[] LPushX = "LPUSHX".ToUtf8Bytes();
        /// <summary>
        /// LLEN key
        /// 获得队列(List)的长度
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 返回存储在 key 里的list的长度。 
        /// 如果 key 不存在，那么就被看作是空list，并且返回长度为 0。 
        /// 当存储在 key 里的值不是一个list的话，会返回error。
        /// 返回值
        /// 整数: key对应的list的长度。
        /// http://www.redis.cn/commands/llen.html
        /// http://redis.readthedocs.org/en/latest/list/llen.html
        /// </summary>
        public readonly static byte[] LLen = "LLEN".ToUtf8Bytes();
        /// <summary>
        /// LRANGE key start stop
        /// 从列表中获取指定返回的元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(S+N)。其中S是起始偏移量，N是指定范围内的元素数量。
        /// 返回存储在 key 的列表里指定范围内的元素。 
        /// start 和 end 偏移量都是基于0的下标，即list的第一个元素下标是0（list的表头），第二个元素下标是1，以此类推。
        /// 偏移量也可以是负数，表示偏移量是从list尾部开始计数。 
        /// 例如， -1 表示列表的最后一个元素，-2 是倒数第二个，以此类推。
        /// http://www.redis.cn/commands/lrange.html
        /// http://redis.readthedocs.org/en/latest/list/lrange.html
        /// </summary>
        public readonly static byte[] LRange = "LRANGE".ToUtf8Bytes();
        /// <summary>
        /// LTRIM key start stop
        /// 修剪到指定范围内的清单
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N) 这里的N是该操作中要被移除掉的元素个数。
        /// http://www.redis.cn/commands/ltrim.html
        /// http://redis.readthedocs.org/en/latest/list/ltrim.html
        /// </summary>
        public readonly static byte[] LTrim = "LTRIM".ToUtf8Bytes();
        /// <summary>
        /// LINDEX key index
        /// 获取一个元素，通过其索引列表
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N)，此处N是通过下标遍历到要找的元素所经过元素个数。
        /// 也就是说要询问list里的第一个或者最后一个元素的复杂度是O(1)。
        /// 返回列表里的元素的索引 index 存储在 key 里面。 
        /// 下标是从0开始索引的，所以 0 是表示第一个元素， 1 表示第二个元素，并以此类推。 
        /// 负数索引用于指定从列表尾部开始索引的元素。在这种方法下，-1 表示最后一个元素，-2 表示倒数第二个元素，并以此往前推。
        /// 当 key 位置的值不是一个列表的时候，会返回一个error。
        /// 返回值
        /// 批量回复：请求的对应元素，或者当 index 超过范围的时候返回 nil。
        /// http://www.redis.cn/commands/lindex.html
        /// http://redis.readthedocs.org/en/latest/list/lindex.html
        /// </summary>
        public readonly static byte[] LIndex = "LINDEX".ToUtf8Bytes();
        /// <summary>
        /// LINSERT key BEFORE|AFTER pivot value
        /// 在列表中的另一个元素之前或之后插入一个元素
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(N)，此处N是在看见 pivot 值之前遍历过的元素个数。 
        /// 这意味着在list最左端（表头）进行插入的复杂度是O(1)，而在list最右端（表尾）进行插入是O(N)。
        /// 把 value 插入存于 key 的列表中在基准值 pivot 的前面或后面。
        /// 当 key 不存在时，这个list会被看作是空list，任何操作都不会发生。
        /// 当 key 存在，但保存的不是一个list的时候，会返回error。
        /// 返回值
        /// 整型回复: 经过插入操作后的list长度，或者当 pivot 值找不到的时候返回 -1。
        /// http://www.redis.cn/commands/linsert.html
        /// http://redis.readthedocs.org/en/latest/list/linsert.html
        /// </summary>
        public readonly static byte[] LInsert = "LINSERT".ToUtf8Bytes();
        /// <summary>
        /// LINSERT的BEFORE选项
        /// http://www.redis.cn/commands/linsert.html
        /// http://redis.readthedocs.org/en/latest/list/linsert.html
        /// </summary>
        public readonly static byte[] Before = "BEFORE".ToUtf8Bytes();
        /// <summary>
        /// LINSERT的AFTER选项
        /// http://www.redis.cn/commands/linsert.html
        /// http://redis.readthedocs.org/en/latest/list/linsert.html
        /// </summary>
        public readonly static byte[] After = "AFTER".ToUtf8Bytes();
        /// <summary>
        /// LSET key index value
        /// 设置队列里面一个元素的值
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N)，此处N表示list的长度。设置list里第一个或者最后一个元素的复杂度是O(1)。
        /// 设置 index 位置的list元素的值为 value。 更多关于 index 参数的信息，详见 LINDEX。
        /// 当index超出范围时会返回一个error。
        /// 返回值
        /// 状态回复
        /// http://www.redis.cn/commands/lset.html
        /// http://redis.readthedocs.org/en/latest/list/lset.html
        /// </summary>
        public readonly static byte[] LSet = "LSET".ToUtf8Bytes();
        /// <summary>
        /// LREM key count value
        /// 从列表中删除元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(N)，此处N表示list的长度。
        /// 从存于 key 的列表里移除前 count 次出现的值为 value 的元素。 这个 count 参数通过下面几种方式影响这个操作：
        /// count > 0: 从头往尾移除值为 value 的元素。
        /// count < 0: 从尾往头移除值为 value 的元素。
        /// count = 0: 移除所有值为 value 的元素。
        /// 比如， LREM list -2 "hello" 会从存于 list 的列表里移除最后两个出现的 "hello"。
        /// 需要注意的是，如果list里没有存在key就会被当作空list处理，所以当 key 不存在的时候，这个命令会返回 0。
        /// 返回值
        /// 整型回复: 被移除的元素个数。
        /// http://www.redis.cn/commands/lrem.html
        /// http://redis.readthedocs.org/en/latest/list/lrem.html
        /// </summary>
        public readonly static byte[] LRem = "LREM".ToUtf8Bytes();
        /// <summary>
        /// LPOP key
        /// 从队列的左边出队一个元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 移除并且返回 key 对应的 list 的第一个元素。
        /// 返回值
        /// 批量回复: 返回第一个元素的值，或者当 key 不存在时返回 nil。
        /// http://www.redis.cn/commands.html
        /// http://redis.readthedocs.org/en/latest/list/lpop.html
        /// </summary>
        public readonly static byte[] LPop = "LPOP".ToUtf8Bytes();
        /// <summary>
        /// RPOP key
        /// 从队列的右边出队一个元素
        /// 加入版本 1.0.0。
        /// 时间复杂度： O(1)。
        /// 移除并返回存于 key 的 list 的最后一个元素。
        /// 返回值
        /// 批量回复: 最后一个元素的值，或者当 key 不存在的时候返回 nil。
        /// http://www.redis.cn/commands/rpop.html
        /// http://redis.readthedocs.org/en/latest/list/rpop.html
        /// </summary>
        public readonly static byte[] RPop = "RPOP".ToUtf8Bytes();
        /// <summary>
        /// BLPOP key [key ...] timeout
        /// 删除，并获得该列表中的第一元素，或阻塞，直到有一个可用
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// BLPOP 是阻塞式列表的弹出原语。 
        /// 它是命令 LPOP 的阻塞版本，这是因为当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞。
        /// 当给定多个 key 参数时，按参数 key 的先后顺序依次检查各个列表，弹出第一个非空列表的头元素。
        /// http://www.redis.cn/commands/blpop.html
        /// http://redis.readthedocs.org/en/latest/list/blpop.html
        /// </summary>
        public readonly static byte[] BLPop = "BLPOP".ToUtf8Bytes();
        /// <summary>
        /// BRPOP key [key ...] timeout
        /// 删除，并获得该列表中的最后一个元素，或阻塞，直到有一个可用
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(1)。
        /// BRPOP 是一个阻塞的列表弹出原语。 
        /// 它是 RPOP 的阻塞版本，因为这个命令会在给定list无法弹出任何元素的时候阻塞连接。 
        /// 该命令会按照给出的 key 顺序查看 list，并在找到的第一个非空 list 的尾部弹出一个元素。
        /// 请在 BLPOP 文档 中查看该命令的准确语义，因为 BRPOP 和 BLPOP 基本是完全一样的，除了它们一个是从尾部弹出元素，而另一个是从头部弹出元素。
        /// 返回值
        /// 多批量回复: 具体来说:
        /// 当没有元素可以被弹出时返回一个 nil 的多批量值，并且 timeout 过期。
        /// 当有元素弹出时会返回一个双元素的多批量值，其中第一个元素是弹出元素的 key，第二个元素是 value。
        /// http://www.redis.cn/commands/brpop.html
        /// http://redis.readthedocs.org/en/latest/list/brpop.html
        /// </summary>
        public readonly static byte[] BRPop = "BRPOP".ToUtf8Bytes();
        /// <summary>
        /// RPOPLPUSH source destination
        /// 删除列表中的最后一个元素，将其追加到另一个列表
        /// 加入版本 1.2.0。
        /// 时间复杂度： O(1)。
        /// 原子性地返回并移除存储在 source 的列表的最后一个元素（列表尾部元素），并把该元素放入存储在 destination 的列表的第一个元素位置（列表头部）。
        /// 例如：假设 source 存储着列表 a,b,c， destination存储着列表 x,y,z。
        /// 执行 RPOPLPUSH 得到的结果是 source 保存着列表 a,b ，而 destination 保存着列表 c,x,y,z。
        /// 如果 source 不存在，那么会返回 nil 值，并且不会执行任何操作。 
        /// 如果 source 和 destination 是同样的，那么这个操作等同于移除列表最后一个元素并且把该元素放在列表头部， 所以这个命令也可以当作是一个旋转列表的命令。
        /// 返回值
        /// 批量回复: 被移除和放入的元素
        /// http://www.redis.cn/commands/rpoplpush.html
        /// http://redis.readthedocs.org/en/latest/list/rpoplpush.html
        /// </summary>
        public readonly static byte[] RPopLPush = "RPOPLPUSH".ToUtf8Bytes();
        /// <summary>
        /// BRPOPLPUSH source destination timeout
        /// 弹出一个列表的值，将它推到另一个列表，并返回它;或阻塞，直到有一个可用
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// BRPOPLPUSH 是 RPOPLPUSH 的阻塞版本。 
        /// 当 source 包含元素的时候，这个命令表现得跟 RPOPLPUSH 一模一样。 
        /// 当 source 是空的时候，Redis将会阻塞这个连接，直到另一个客户端 push 元素进入或者达到 timeout 时限。
        /// timeout 为 0 能用于无限期阻塞客户端。
        /// 查看 RPOPLPUSH 以了解更多信息。
        /// 返回值
        /// 批量回复: 元素从 source 中弹出来，并压入 destination 中。 
        /// 如果达到 timeout 时限，会返回一个 空的多批量回复。
        /// http://www.redis.cn/commands/brpoplpush.html
        /// http://redis.readthedocs.org/en/latest/list/brpoplpush.html
        /// </summary>
        public readonly static byte[] BRPopLPush = "BRPOPLPUSH".ToUtf8Bytes();
        #endregion

        #region Transaction（事务）
        /// <summary>
        /// WATCH key [key ...]
        /// 锁定key直到执行了 MULTI/EXEC 命令
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1) for every key。
        /// 标记所有指定的key 被监视起来，在事务中有条件的执行（乐观锁）。
        /// 返回值
        /// 状态应答码: 总是 OK。
        /// http://www.redis.cn/commands/watch.html
        /// http://redis.readthedocs.org/en/latest/transaction/watch.html
        /// </summary>
        public readonly static byte[] Watch = "WATCH".ToUtf8Bytes();
        /// <summary>
        /// UNWATCH 
        /// 取消事务
        /// 加入版本 2.2.0。
        /// 时间复杂度： O(1)。
        /// 刷新一个事务中已被监视的所有key。
        /// 如果执行EXEC 或者DISCARD， 则不需要手动执行UNWATCH 。
        /// 返回值
        /// 状态应答码: 总是 OK。
        /// http://www.redis.cn/commands/unwatch.html
        /// http://redis.readthedocs.org/en/latest/transaction/unwatch.html
        /// </summary>
        public readonly static byte[] UnWatch = "UNWATCH".ToUtf8Bytes();
        /// <summary>
        /// MULTI 
        /// 标记一个事务块开始
        /// 加入版本 1.2.0。
        /// 标记一个事务块的开始。 随后的指令将在执行EXEC时作为一个原子执行。
        /// 返回值
        /// 状态应答码: 始终为OK
        /// http://www.redis.cn/commands/multi.html
        /// http://redis.readthedocs.org/en/latest/transaction/multi.html
        /// </summary>
        public readonly static byte[] Multi = "MULTI".ToUtf8Bytes();
        /// <summary>
        /// EXEC 
        /// 执行所有 MULTI 之后发的命令
        /// 加入版本 1.2.0。
        /// 执行事务中所有在排队等待的指令并将链接状态恢复到正常
        /// 当使用WATCH 时，只有当被监视的键没有被修改，且允许检查设定机制时，EXEC会被执行
        /// 返回值
        /// 应答集合: 每个元素与原子事务中的指令一一对应
        /// 当使用WATCH时，如果被终止，EXEC 则返回一个空的应答集合
        /// http://www.redis.cn/commands/exec.html
        /// http://redis.readthedocs.org/en/latest/transaction/exec.html
        /// </summary>
        public readonly static byte[] Exec = "EXEC".ToUtf8Bytes();
        /// <summary>
        /// DISCARD 
        /// 丢弃所有 MULTI 之后发的命令
        /// 加入版本 2.0.0。
        /// 刷新一个事务中所有在排队等待的指令，并且将连接状态恢复到正常。
        /// 如果 已使用 WATCH，DISCARD 将释放所有被WATCH的key。
        /// 返回值
        /// 应答状态码：所有返回都是 OK
        /// http://www.redis.cn/commands/discard.html
        /// http://redis.readthedocs.org/en/latest/transaction/discard.html
        /// </summary>
        public readonly static byte[] Discard = "DISCARD".ToUtf8Bytes();
        #endregion

        #region Pub/Sub（发布/订阅）
        /// <summary>
        /// SUBSCRIBE channel [channel ...]
        /// 聆听发布途径的消息
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N) N 是订阅的频道的数量。
        /// 订阅给指定频道的信息。
        /// http://www.redis.cn/commands/subscribe.html
        /// http://redis.readthedocs.org/en/latest/pub_sub/subscribe.html
        /// </summary>
        public readonly static byte[] Subscribe = "SUBSCRIBE".ToUtf8Bytes();
        /// <summary>
        /// UNSUBSCRIBE [channel [channel ...]]
        /// 停止发布途径的消息听
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N) where N is the number of clients already subscribed to a channel。
        /// http://www.redis.cn/commands/unsubscribe.html
        /// http://redis.readthedocs.org/en/latest/pub_sub/unsubscribe.html
        /// </summary>
        public readonly static byte[] UnSubscribe = "UNSUBSCRIBE".ToUtf8Bytes();
        /// <summary>
        /// PSUBSCRIBE pattern [pattern ...]
        /// 听出版匹配给定模式的渠道的消息
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N) ， N 是订阅的模式的数量。
        /// 订阅给定的模式。
        /// http://www.redis.cn/commands/psubscribe.html
        /// http://redis.readthedocs.org/en/latest/pub_sub/psubscribe.html
        /// </summary>
        public readonly static byte[] PSubscribe = "PSUBSCRIBE".ToUtf8Bytes();
        /// <summary>
        /// PUNSUBSCRIBE [pattern [pattern ...]]
        /// 停止发布到匹配给定模式的渠道的消息听
        /// 加入版本 2.0.0。
        /// 时间复杂度： O(N+M) where N is the number of patterns the client is already subscribed and M is the number of total patterns subscribed in the system (by any client)。
        /// http://www.redis.cn/commands/punsubscribe.html
        /// http://redis.readthedocs.org/en/latest/pub_sub/punsubscribe.html
        /// </summary>
        public readonly static byte[] PUnSubscribe = "PUNSUBSCRIBE".ToUtf8Bytes();
        /// <summary>
        /// PUBLISH channel message
        /// 发布一条消息到频道加入版本 2.0.0。
        /// 时间复杂度： O(N+M) 其中 N 是频道 channel 的订阅者数量，而 M 则是使用模式订阅(subscribed patterns)的客户端的数量。
        /// 将信息 message 发送到指定的频道 channel
        /// 返回值
        /// Integer reply: 收到消息的客户端数量。
        /// http://www.redis.cn/commands/publish.html
        /// http://redis.readthedocs.org/en/latest/pub_sub/publish.html
        /// </summary>
        public readonly static byte[] Publish = "PUBLISH".ToUtf8Bytes();
        #endregion

        #region 选项关键字
        /// <summary>
        /// ZRANGE key start stop [WITHSCORES]
        /// ZRANGEBYSCORE key min max [WITHSCORES] [LIMIT offset count]
        /// ZREVRANGE key start stop [WITHSCORES]
        /// ZREVRANGEBYSCORE key max min [WITHSCORES] [LIMIT offset count]
        /// </summary>
        public readonly static byte[] WithScores = "WITHSCORES".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// ZRANGEBYSCORE key min max [WITHSCORES] [LIMIT offset count]
        /// ZREVRANGEBYSCORE key max min [WITHSCORES] [LIMIT offset count]
        /// </summary>
        public readonly static byte[] Limit = "LIMIT".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// </summary>
        public readonly static byte[] By = "BY".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// </summary>
        public readonly static byte[] Asc = "ASC".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// </summary>
        public readonly static byte[] Desc = "DESC".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// </summary>
        public readonly static byte[] Alpha = "ALPHA".ToUtf8Bytes();
        /// <summary>
        /// SORT key [BY pattern] [LIMIT offset count] [GET pattern [GET pattern ...]] [ASC|DESC] [ALPHA] [STORE destination]
        /// </summary>
        public readonly static byte[] Store = "STORE".ToUtf8Bytes();
        #endregion

        #region Script（脚本）
        /// <summary>
        /// EVAL script numkeys key [key ...] arg [arg ...]
        /// 在服务器端执行 LUA 脚本
        /// 加入版本 2.6.0。
        /// 时间复杂度： Depends on the script that is executed。
        /// http://www.redis.cn/commands/eval.html
        /// http://redis.readthedocs.org/en/latest/script/eval.html
        /// </summary>
        public readonly static byte[] Eval = "EVAL".ToUtf8Bytes();
        /// <summary>
        /// EVALSHA sha1 numkeys key [key ...] arg [arg ...]
        /// 在服务器端执行 LUA 脚本
        /// 加入版本 2.6.0。
        /// 时间复杂度： 取决于所执行的脚本。
        /// 根据给定的 SHA1 校验码，对缓存在服务器中的脚本进行求值。
        /// 将脚本缓存到服务器的操作可以通过 SCRIPT LOAD 命令进行。 
        /// 这个命令的其他地方，比如参数的传入方式，都和 EVAL 命令一样。
        /// http://www.redis.cn/commands/evalsha.html
        /// http://redis.readthedocs.org/en/latest/script/evalsha.html
        /// </summary>
        public readonly static byte[] EvalSha = "EVALSHA".ToUtf8Bytes();
        /// <summary>
        /// 脚本操作的前缀
        /// </summary>
        public readonly static byte[] Script = "SCRIPT".ToUtf8Bytes();
        /// <summary>
        /// SCRIPT LOAD script
        /// 从服务器缓存中装载一个Lua脚本。
        /// 加入版本 2.6.0。
        /// 时间复杂度： O(N) , N 为脚本的长度(以字节为单位)。
        /// 将脚本 script 添加到脚本缓存中，但并不立即执行这个脚本。 
        /// EVAL 命令也会将脚本添加到脚本缓存中，但是它会立即对输入的脚本进行求值。
        /// 在脚本被加入到缓存之后，通过 EVALSHA 命令，可以使用脚本的 SHA1 校验和来调用这个脚本。
        /// 脚本可以在缓存中保留无限长的时间，直到执行 SCRIPT FLUSH 为止。
        /// 如果添加的脚本已经存在于脚本缓存，那么不做任何动作。
        /// 关于使用 Redis 对 Lua 脚本进行求值的更多信息，请参见 EVAL 命令。
        /// 返回值
        /// Bulk reply 给定 script 的 SHA1 校验值
        /// http://www.redis.cn/commands/script-load.html
        /// http://redis.readthedocs.org/en/latest/script/script_load.html
        /// </summary>
        public readonly static byte[] Load = "LOAD".ToUtf8Bytes();
        /// <summary>
        /// SCRIPT FLUSH 
        /// 删除服务器缓存中所有Lua脚本。
        /// 加入版本 2.6.0。
        /// 时间复杂度： O(N) ， N 为缓存中脚本的数量。
        /// 清除所有 Lua 脚本缓存。
        /// 关于使用 Redis 对 Lua 脚本进行求值的更多信息，请参见 EVAL 命令。
        /// 返回值
        /// Status code reply
        /// http://www.redis.cn/commands/script-flush.html
        /// http://redis.readthedocs.org/en/latest/script/script_flush.html
        /// </summary>
        public readonly static byte[] Flush = "FLUSH".ToUtf8Bytes();
        #endregion

        public readonly static byte[] Ex = "EX".ToUtf8Bytes();
        public readonly static byte[] Px = "PX".ToUtf8Bytes();
        public readonly static byte[] Nx = "NX".ToUtf8Bytes();
        public readonly static byte[] Xx = "XX".ToUtf8Bytes();

        /// <summary>
        /// Sentinel commands
        /// </summary>
        public readonly static byte[] Sentinel = "SENTINEL".ToUtf8Bytes();
    }
}
