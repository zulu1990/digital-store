using Application.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class RegisterApplicationServices
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());


            //services.AddScoped(typeof(IPipelineBehavior<,>),
            //                    typeof(ValidatorBehavior<,>));

            return services;
        }
    }
}
