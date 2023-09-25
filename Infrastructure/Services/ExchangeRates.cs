using Application.Services;
using Application.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace Infrastructure.Services
{
    public class ExchangeRates : IExchangeRate
    {
        private readonly ICacheService _cacheService;
        private readonly ExchangeRateConfig _config;



        public ExchangeRates(ICacheService cacheService, IOptions<ExchangeRateConfig> config)
        {
            _cacheService = cacheService;
            _config = config.Value;
        }

        public async Task<ExchangeRateResponse> GetExchangeRates(string currency)
        {
            var data = _cacheService.GetData<ExchangeRateResponse>(currency);

            if(data == null)
            {
                var url = string.Concat(_config.BaseUrl, _config.ApiKey, _config.Param, currency);

                using var httpClient = new HttpClient();

                data = await httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

                

                _cacheService.SetData(currency, data);
            }

            return data;
        }
    }
}
