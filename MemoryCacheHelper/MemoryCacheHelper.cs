using System;
using System.Runtime.Caching;

namespace MemoryCacheProvider
{
    /// <summary>
    /// 基于Memory的缓存机制
    /// </summary>
    public class MemoryCacheHelper
    {
        private static readonly object _addLocker = new object();

        /// <summary>
        /// 获取或添加缓存节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cachePopulate"></param>
        /// <returns></returns>
        public static T GetOrSetCacheItem<T>(string key, Func<T> cachePopulate) 
        {
            var slidingExpiration = new TimeSpan(0, 2, 0);

            return GetOrSetCacheItem(key, cachePopulate, slidingExpiration);
        }

        /// <summary>
        /// 获取或添加缓存节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cachePopulate"></param>
        /// <param name="slidingExpiration"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        public static T GetOrSetCacheItem<T>(string key, Func<T> cachePopulate, TimeSpan slidingExpiration, DateTimeOffset? absoluteExpiration = null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Invalid cache key");
            if (cachePopulate == null) throw new ArgumentNullException("cachePopulate");
            if (slidingExpiration == null)
                throw new ArgumentException("Eiter a sliding expiration must be provider");

            if (MemoryCache.Default[key] == null)
            {
                lock (_addLocker)
                {
                    if (MemoryCache.Default[key] == null)
                    {
                        var cacheItem = new CacheItem(key, cachePopulate());
                        var policy = new CacheItemPolicy
                        {
                            SlidingExpiration = slidingExpiration, //指定时长未被访问
                            //AbsoluteExpiration = absoluteExpiration.Value, //过期时间
                        };
                        MemoryCache.Default.Add(cacheItem, policy);
                    }
                }
            }

            return (T)MemoryCache.Default[key];
        }

        public static T GetOrRemoveCacheItem<T>(string key) 
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Invalid cache key");

            return (T)MemoryCache.Default[key];
        }
    }
}
