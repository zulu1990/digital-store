using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Orders.Models
{
    public class AddProductToOrderModel
    {
        public int ProductIdentifier { get; set; }
        public int Count { get; set; }
    }
}
