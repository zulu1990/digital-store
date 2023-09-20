using Application.Orders.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    public class OrderController : BaseController
    {
        public OrderController(ISender mediator) : base(mediator)
        {
        }


        //[Authorize]
        [HttpPost("add-product-to-order")]
        public async Task<IActionResult> AddProductToOrder(AddProductToOrderModel model)
        {

        }

    }
}
