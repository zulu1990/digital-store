using Application.Orders.Commands;
using Application.Orders.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Extensions;

namespace OnlineStore.Controllers
{
    public class OrderController : BaseController
    {
        public OrderController(ISender mediator) : base(mediator)
        {
        }


        [Authorize]
        [HttpPost("add-product-to-order")]
        public async Task<IActionResult> AddProductToOrder(AddProductToOrderModel model)
        {
            var userId = HttpContext.GetUserId();

            var addProductToOrderCommand = new AddProductToOrderCommand(userId, model.ProductIdentifier, model.Count);

            var result = await _mediator.Send(addProductToOrderCommand);

            return Ok(result);
        }


        [Authorize]
        [HttpPost("remove-product-from-order")]
        public async Task<IActionResult> RemoveProductFromOrder(RemoveProductFromOrder model)
        {
            var userId = HttpContext.GetUserId();

            var removeProductFromOrderCommand = new RemoveProductFromOrderCommand(userId, model.ProductIdentifier, model.Count);

            var result = await _mediator.Send(removeProductFromOrderCommand);

            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckoutOrder()
        {
            var userId = HttpContext.GetUserId();
            var checkoutOrderCommand = new CheckoutOrderCommand(userId);

            var result = await _mediator.Send(checkoutOrderCommand);

            return Ok(result);
        }
    }
}
