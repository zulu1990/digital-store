using Domain;
using MediatR;
using OnlineStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.Queries
{
    public record GetWeatherQuery : IRequest<Result<IEnumerable<WeatherForecast>>>;


    internal class GetWeatherQueryHander : IRequestHandler<GetWeatherQuery, Result<IEnumerable<WeatherForecast>>>
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", 
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public async Task<Result<IEnumerable<WeatherForecast>>> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
        {
            var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            await Task.CompletedTask;

            return Result<IEnumerable<WeatherForecast>>.Succeed(data);
        }
    }


}
