using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Models
{
    public class GetUserInfoResponseModel
    {
        public Guid UserId { get; internal set; }
        public decimal Balance { get; internal set; }
        public ICollection<Order> Orders { get; internal set; }
        public string Currency { get; internal set; }
        public string Email { get; internal set; }
    }
}
