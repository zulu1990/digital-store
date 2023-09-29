using Application.Common.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposed;
        private ApplicationDbContext _dbContext;
        private readonly Func<ApplicationDbContext> _instanceFactory;

        public ApplicationDbContext DbContext => _dbContext ??= _instanceFactory.Invoke();

        public UnitOfWork(Func<ApplicationDbContext> dbContextFactory)
        {
            _instanceFactory = dbContextFactory;
        }

        public async Task<bool> CommitAsync()
        {
            return await DbContext.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            if (_disposed == false && _dbContext != null)
            {
                _disposed = true;
                _dbContext.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
