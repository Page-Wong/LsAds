using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace LsAdmin.Utility.Convert
{
    public class CacheHelper
    {
        static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCacheValue(string key) {
            object val = null;
            if (key != null && cache.TryGetValue(key, out val)) {                
                return val;
            }
            else {
                return default(object);
            }
        }
        /// <summary>
        /// 添加缓存内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetChacheValue(string key, object value) {
            if (key != null) {
                cache.Set(key, value, new MemoryCacheEntryOptions {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveCache(string key) {
            if (key != null) {
                cache.Remove(key);
            }
        }
    }
}
