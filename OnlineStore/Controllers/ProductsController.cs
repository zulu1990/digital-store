using Application.Comments.Commands;
using Application.Comments.Models.Input;
using Application.Products.Models;
using Application.Products.Query;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Extensions;

namespace OnlineStore.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(ISender mediator, ILoggerFactory loggerFactory) : base(mediator, loggerFactory)
        {
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllProducts()
        {
            var userId = HttpContext.GetUserId();
            _logger.LogDebug($"GetAllProducts for user :{userId}");

            var result = await _mediator.Send(new GetAllProductsQuery(userId));
            return Ok(result);
        }


        [HttpGet("get-products-by-productIdentifier/{productIdentifier}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProductsByCategory(int productIdentifier)
        {
            var userId = HttpContext.GetUserId();

            var query = new GetProductsByCategoryQuery(productIdentifier, userId);

            var result = await _mediator.Send(query);

            return Ok(result);
        }




        [HttpPost("add-comment-to-product")]
        public async Task<IActionResult> AddComment(AddCommentModel addCommentModel)
        {
            var userId = HttpContext.GetUserId();

            var command = new AddCommentToProductCommand(userId, addCommentModel.Message,
                addCommentModel.Rating, addCommentModel.ProductIdentifier);

            var result = await _mediator.Send(command);

            return Ok(result);
        }


        [HttpDelete("delete-comment")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = HttpContext.GetUserId();
            var command = new DeleteCommentCommand(userId, commentId);

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPut("edit-comment")]
        public async Task<IActionResult> EditComment(EditCommentModel editCommand)
        {
            var userId = HttpContext.GetUserId();

            var command = new CommentEditCommand(userId, editCommand.CommentId,
                editCommand.Message, editCommand.Rating);

             var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
