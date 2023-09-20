using Application.Common.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        public Task<bool> CommitAsync()
        {
            throw new NotImplementedException();
        }
    }
}
