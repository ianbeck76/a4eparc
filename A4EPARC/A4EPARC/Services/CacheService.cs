using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace A4EPARC.Services
{

    public class CacheService : ICacheService
    {
        public void Add(string key, object obj)
        {
            HttpContext.Current.Cache.Add(key, obj, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                                          CacheItemPriority.Default, null);
        }

        public void Add(string key, object obj, DateTime expiration)
        {
            HttpContext.Current.Cache.Add(key, obj, null, expiration, Cache.NoSlidingExpiration,
                                          CacheItemPriority.Default, null);
        }

        public void Insert(string key, object obj)
        {
            HttpContext.Current.Cache.Insert(key, obj, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                                             CacheItemPriority.Default, null);
        }

        public void InsertPerRequest(string key, object obj)
        {
            HttpContext.Current.Items[key] = obj;
        }

        public void Insert(string key, object obj, DateTime expiration)
        {
            HttpContext.Current.Cache.Insert(key, obj, null, expiration, Cache.NoSlidingExpiration,
                                             CacheItemPriority.Default, null);
        }

        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public T GetPerRequest<T>(string key)
        {
            return (T) HttpContext.Current.Items[key];
        }

        public object Get(string key)
        {
            return HttpContext.Current.Cache[key];
        }

        public T Get<T>(string key)
        {
            return (T) HttpContext.Current.Cache[key];
        }

        public List<string> Keys()
        {
            var keys = (from System.Collections.DictionaryEntry dictionaryEntry in HttpContext.Current.Cache
                        select dictionaryEntry.Key.ToString()).ToList();
            return keys;
        }

        public void RemoveStartsWith(string keyStartsWith)
        {
            var keys = from key in Keys() where key.ToLower().StartsWith(keyStartsWith.ToLower()) select key;
            foreach (var key in keys)
            {
                Remove(key);
            }
        }
    }

    public interface ICacheService
    {
        void Add(string key, object obj);
        void Add(string key, object obj, DateTime expiration);
        void Insert(string key, object obj);
        void Insert(string key, object obj, DateTime expiration);
        void InsertPerRequest(string key, object obj);
        void Remove(string key);
        T Get<T>(string key);
        T GetPerRequest<T>(string key);
        object Get(string key);
        List<string> Keys();
        void RemoveStartsWith(string keyStartsWith);
    }
}