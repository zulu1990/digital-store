using Application.Weather.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    public class WeatherForecastController : BaseController
    {

        public WeatherForecastController(ISender mediator) : base(mediator) { }




        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), 200)]
        [ProducesResponseType(404)]
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            var getWeatherQuery = new GetWeatherQuery();
            var result = await _mediator.Send(getWeatherQuery);

            return Ok(result.Value);

        }
    }
}