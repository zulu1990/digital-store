using Microsoft.AspNetCore.Http;

namespace Application.Images.Models
{
    public record UploadImageModel(IFormFile File, int ProductIdentifier, string AdminSecret);
}
