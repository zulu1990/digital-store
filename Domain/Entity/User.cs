using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public decimal Balance { get; set; }

        public string Currency { get; set; } = "EUR";

        public ICollection<Order> Orders { get; set; }

    }
}
