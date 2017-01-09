using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheAside
{
    // Testing this pattern:
    // https://msdn.microsoft.com/en-us/library/dn589799.aspx

    class DataSource
    {
        ICacheManager<object> cache;
        Dictionary<string, object> dataStore;

        public DataSource()
        {
            //runtime cachhandle not set up for dotnet core
            cache = CacheFactory.Build("getStartedCache", settings =>
            {
                //settings.WithSystemRuntimeCacheHandle("handleName");
            });

            dataStore = new Dictionary<string, object>();
            dataStore.Add("test1", "test1test");
            dataStore.Add("test2", "test2test");
            dataStore.Add("test3", "test3test");
        }

        public bool ValueExists(string key)
        {
            return dataStore.ContainsKey(key);
        }

        public Dictionary<string, object> GetDataValues()
        {
            return dataStore;
        }

        public object Get(string key)
        {
            bool cacheException = false;

            try
            {
                // Try to get the entity from the cache.
                var cacheItem = cache.GetCacheItem(key);
                if (cacheItem != null)
                {
                    Console.WriteLine("Datasource: Cache");
                    return cacheItem.Value;
                }
            }
            catch
            {
                // If there is a cache related issue, raise an exception 
                // and avoid using the cache for the rest of the call.
                cacheException = true;
            }

            // If there is a cache miss, get the entity from the original store and cache it.
            // Code has been omitted because it is data store dependent.  
            var value = dataStore[key];

            if (!cacheException)
            {
                try
                {
                    // Avoid caching a null value.
                    if (value != null)
                    {
                        // Put the item in the cache with a custom expiration time that 
                        // depends on how critical it might be to have stale data.
                        var item = new CacheItem<object>(key, value, ExpirationMode.Absolute, TimeSpan.FromSeconds(15));
                        cache.Put(item);

                        Console.WriteLine("Datasource: Datastore");
                    }
                }
                catch
                {
                    // If there is a cache related issue, ignore it
                    // and just return the entity.
                }
            }

            return value;
        }

        // As per MSDN cache pattern, if an application updates information, it can emulate the write-through strategy as follows:
        //    Make the modification to the data store
        //    Invalidate the corresponding item in the cache.

        public bool Update(string key, string value)
        {
            try
            {
                cache.Remove(key);
            }
            catch
            {
                // If there is a cache related issue, ignore it. In this case, if there is a cache exception, the cache value will expire in 15 seconds.
            }

            if (dataStore.ContainsKey(key))
            {
                dataStore[key] = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
