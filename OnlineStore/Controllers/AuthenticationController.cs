using Application.Authorization.Commands;
using Application.Authorization.Commands.Passwords;
using Application.Authorization.Model;
using Application.Authorization.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineStore.Extensions;

namespace OnlineStore.Controllers
{
    public class AuthenticationController : BaseController
    {
        public AuthenticationController(ISender mediator, ILoggerFactory loggerFactory) : base(mediator, loggerFactory) { }


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


        [HttpGet("verify")]
        public async Task<IActionResult> Verify(string email, Guid verificationCode)
        {
            var command = new VerifyUserEmailCommand(email, verificationCode);

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok();

            return BadRequest();
        }


        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPassword)
        {
            var command = new ForgotPasswordCommand(forgetPassword.Email);

            var result = await _mediator.Send(command);

            return Ok(result);
        }


        [HttpPost("reset-passsord")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPassword)
        {
            var command = new ResetPasswordCommand(resetPassword.Email, resetPassword.Password, resetPassword.Token);

            var result = await _mediator.Send(command);

            return Ok(result);
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = HttpContext.GetUserId();

            var command = new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
