using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Models
{
    public class UserBanModel
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; }

    }
}
