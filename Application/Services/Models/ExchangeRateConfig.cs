using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Models
{
    public class ExchangeRateConfig
    {
        public string BaseUrl { get; set; }
        public string Currency { get; set; }
        public string ApiKey { get; set; }
        public string Param { get; set; }
    }
}
