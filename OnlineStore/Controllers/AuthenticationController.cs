using Application.Authorization.Commands;
using Application.Authorization.Model;
using Application.Authorization.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    public class AuthenticationController : BaseController
    {
        public AuthenticationController(ISender mediator/*, ILogger logger*/) : base(mediator/*, logger*/) { }


        [HttpPost("Register")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var command = new RegisterUserCommand(request.Email, request.Password);

            var result = await _mediator.Send(command);

            if (result.Success)
                return Ok(result.Value);
            else
                return Problem(result.Message);
        }


        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var loginQuery = new LoginQuery(login.Email, login.Password);

            var result = await _mediator.Send(loginQuery);

            if (result.Success)
                return Ok(result.Value);
            else
                return Problem(result.Message);
        }

    }
}
