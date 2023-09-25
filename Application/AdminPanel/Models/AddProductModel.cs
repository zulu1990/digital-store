using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Models
{
    public class AddProductModel : AdminRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ProductCategory Category { get; set; }
        public int ProductIdentifier { get; set; }

        public int Count {  get; set; }
    }
}
