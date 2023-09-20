using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public Task<T> AddAsync(T value)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }

        public Result DeleteEntitiy(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteEntityById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByExpressionAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0)
        {
            throw new NotImplementedException();
        }

        public Result<T> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
