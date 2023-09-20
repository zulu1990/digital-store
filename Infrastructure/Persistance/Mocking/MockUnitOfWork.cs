using Application.Common.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Mocking
{
    public class MockUnitOfWork : IUnitOfWork
    {
        public async Task<bool> CommitAsync()
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
