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
        public AdminController(ISender mediator, ILoggerFactory loggerFactory) : base(mediator, loggerFactory)
        {
        }




        //[Authorize(Roles = "Admin")]
        [AdminFilter]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductModel model)
        {
            
            var addCommand = new AddProductCommand(model.Name, model.Price, model.Image,
                model.Category, model.ProductIdentifier, model.Count);

            var result = await _mediator.Send(addCommand);

            return Ok(result);
        }

       
        [AdminFilter]
        [HttpPost("get-user-orders")]
        public async Task<IActionResult> GetUserOrders(GetUserOrderModel model)
        {
            var getUserOrdersQuery = new GetUserOrderQuery(model.UserId);


            var result = await _mediator.Send(getUserOrdersQuery);

            return Ok(result);
        }


        [AdminFilter]
        [HttpPost("get-user-info")]
        public async Task<IActionResult> GetUserInfo(GetUserInfoModel model) 
        {
            var getUserQuery = new GetUserInfoQuery(model.UserId);
            var result = await _mediator.Send(getUserQuery);
            return Ok(result);
        }



        [AdminFilter]
        [HttpPost("ban-user")]
        public async Task<IActionResult> BanUser(UserBanModel model)
        {
            var command = new BanUserCommand(model.UserId, model.Reason);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [AdminFilter]
        [HttpPost("unban-user")]
        public async Task<IActionResult> UnBanUser(UserBanModel model)
        {
            var command = new UnBanUserCommand(model.UserId);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

    }
}
