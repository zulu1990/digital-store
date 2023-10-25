using Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace Application.AdminPanel.Models
{
    public class AddProductModel : AdminRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ProductCategory Category { get; set; }
        public int ProductIdentifier { get; set; }
        public IFormFile Image { get;set; }

        public int Count {  get; set; }
    }
}
