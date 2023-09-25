using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Models
{
    public abstract class AdminRequest
    {
        public string AdminSecret { get; set; }
    }
}
