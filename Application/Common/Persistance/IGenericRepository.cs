using Domain;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Persistance
{
    public interface IGenericRepository <T> where T : BaseEntity
    {
        Task<T> AddAsync(T value);
        Task AddRangeAsync(IEnumerable<T> values);

        Result DeleteEntitiy(T entity);
        Task<Result> DeleteEntityById(Guid id);
        Result<T> Update(T entity);
        Task<Result<T>> GetByIdAsync(Guid id);

        Task<T> GetByExpressionAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = false);

        Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression, string includes = null,
            bool trackChanges = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int count = 0);

    }
}
