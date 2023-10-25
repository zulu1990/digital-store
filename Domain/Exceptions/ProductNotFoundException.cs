using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public int ProductIdentifier { get; set; }

        public ProductNotFoundException(int productIdentifier)
        {
            ProductIdentifier = productIdentifier;
        }
    }
}
