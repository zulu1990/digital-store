using Application.Orders.Models;
using Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Customer.Models
{
    public class UserProfileResponse
    {
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public bool EmailVerified { get; set; }

        public List<OrderResponse> Orders { get; set; }

        public  Conversion_Rates ExchangeRates { get; set; }

    }
}
