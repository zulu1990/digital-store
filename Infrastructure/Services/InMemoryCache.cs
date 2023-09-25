using Application.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services
{
    public class InMemoryCache : ICacheService
    {
        private readonly IMemoryCache _cache;

        public InMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetData<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void RemoveData(string key)
        {
            _cache.Remove(key);
        }

        public bool SetData<T>(string key, T value, MemoryCacheEntryOptions options = null)
        {
            try
            {
                options ??= new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(25),
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Priority = CacheItemPriority.Normal,
                    Size = 55
                };

                _cache.Set(key, value, options);

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
