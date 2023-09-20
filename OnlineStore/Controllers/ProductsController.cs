using Application.Products.Query;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(ISender mediator) : base(mediator)
        {
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllProducts()
        {
            await Task.CompletedTask;

            var result = await _mediator.Send(new GetAllProductsQuery());
            return Ok(result);
        }


        [HttpGet("get-products-by-category/{category}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProductsByCategory(ProductCategory category)
        {
            var query = new GetProductsByCategoryQuery(category);

            var result = await _mediator.Send(query);

            return Ok(result);
        }


    }
}
