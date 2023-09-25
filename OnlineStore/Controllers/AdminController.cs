using Application.AdminPanel.Commands;
using Application.AdminPanel.Models;
using Application.AdminPanel.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Extensions;

namespace OnlineStore.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IConfiguration _configuration;
        public AdminController(ISender mediator, IConfiguration configuration) : base(mediator)
        {
            _configuration = configuration;
        }



        //[Authorize]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(AddProductModel model)
        {
            await Task.CompletedTask;

            if (model.AdminSecret != _configuration.GetSection("AdminPanel:Secret").Value)
                return BadRequest("Wrong Secret Key");

            var addCommand = new AddProductCommand(model.Name, model.Price, model.Category, model.ProductIdentifier, model.Count);

            var result = await _mediator.Send(addCommand);

            return Ok(result);
        }

        [HttpGet("get-user-orders")]
        public async Task<IActionResult> GetUserOrders(Guid userId)
        {
            var getUserOrdersQuery = new GetUserOrderQuery(userId);

            var result = await _mediator.Send(getUserOrdersQuery);

            return Ok(result);
        }




    }
}
