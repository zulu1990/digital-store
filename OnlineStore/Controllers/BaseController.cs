using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        internal readonly ISender _mediator;
        internal readonly ILogger _logger;

        public BaseController(ISender mediator, ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }
    }
}