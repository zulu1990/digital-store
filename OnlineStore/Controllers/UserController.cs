using Application.Customer.Commands;
using Application.Customer.Models;
using Application.Customer.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Extensions;

namespace OnlineStore.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ISender mediator, ILoggerFactory loggerFactory) : base(mediator, loggerFactory)
        {
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = HttpContext.GetUserId();

            var query = new UserProfileQuery(userId);

            var result = await _mediator.Send(query);

            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditUserInfo(EditUserRequestModel model)
        {
            var userId = HttpContext.GetUserId();

            var command = new EditUserCommand(userId, model.Role, model.Currency, model.Address);

            var result = await _mediator.Send(command);

            return Ok(result);

        }
    }
}
