using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, MemoryCacheEntryOptions options = null);
        void RemoveData(string key);
    }
}
