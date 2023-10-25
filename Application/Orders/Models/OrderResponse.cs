using Application.Products.Models;

namespace Application.Orders.Models
{
    public class OrderResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
