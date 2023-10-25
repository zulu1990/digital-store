using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace OnlineStore.Extensions
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var config = context.HttpContext.RequestServices.GetService<IConfiguration>();

            var secret = config.GetSection("AdminPanel:Secret").Value;
            var model = context.ActionArguments.Values.FirstOrDefault();

            object properyValue = null;

            if (model != null)
            {
                var propertyInfo = model.GetType().GetProperty("AdminSecret");
                if (propertyInfo != null)
                {
                    properyValue = propertyInfo.GetValue(model);

                    if (properyValue != null && properyValue.ToString() == secret)
                    {
                        base.OnActionExecuting(context);
                        return;
                    }
                }

            }
            var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<AdminFilterAttribute>();
            logger.LogError($"Invalid input model, actual {properyValue} expecting:{secret}");

            context.Result = new BadRequestObjectResult("Invalid input model");
        }

    }
}
