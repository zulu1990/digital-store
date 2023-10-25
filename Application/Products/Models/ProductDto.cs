using Application.Comments.Models.Output;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Models
{
    public class ProductDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ProductCategory Category { get; set; }
        public int ProductIdentifier { get; set; }

        public string? Url { get; set; }
        public double AverageRating { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
