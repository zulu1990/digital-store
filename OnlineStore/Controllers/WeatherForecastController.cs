using Application.Services;
using Application.Weather.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    public class WeatherForecastController : BaseController
    {
        private readonly IExchangeRate _exchangeRate;
        public WeatherForecastController(ISender mediator, IExchangeRate exchangeRate, ILoggerFactory loggerFactory) 
            : base(mediator, loggerFactory)
        {
            _exchangeRate = exchangeRate;
        }




        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), 200)]
        [ProducesResponseType(404)]
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get(string currency)
        {
            _logger.LogInformation($"Logging from {nameof(Get)}");

            throw new Exception("Testing exception");
            //var json = await _exchangeRate.GetExchangeRates(currency);

            //var getWeatherQuery = new GetWeatherQuery();
            //var result = await _mediator.Send(getWeatherQuery);

            return Ok();

        }
    }
}