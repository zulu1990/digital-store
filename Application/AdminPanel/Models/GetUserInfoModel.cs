using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Models
{
    public class GetUserInfoModel : AdminRequest
    {
        public Guid UserId { get; set; }
    }
}
