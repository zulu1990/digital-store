using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Handler
{
    public interface IJwtTokenHander
    {
        string CreateToken(User user);
    }
}
