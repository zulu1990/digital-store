using Application.Images;
using Application.Images.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Extensions;

namespace OnlineStore.Controllers
{
    public class PhotoController : BaseController
    {
        public PhotoController(ISender mediator, ILoggerFactory loggerFactory) : base(mediator, loggerFactory)
        {
        }


        [AdminFilter]
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadImageModel model)
        {
            var command = new UploadImageCommand(model.File, model.ProductIdentifier);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

    }
}
