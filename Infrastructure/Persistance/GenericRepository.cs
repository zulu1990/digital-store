using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Persistance
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
        }


        public async Task<T> AddAsync(T value)
        {
            var entity = await _dbSet.AddAsync(value);
            return entity.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> values)
        {
            await _dbSet.AddRangeAsync(values);
        }

        public Result DeleteEntitiy(T entity)
        {
            _ = _dbSet.Remove(entity);
            return Result.Succeed();
        }

        public async Task<Result> DeleteEntityById(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
                return DeleteEntitiy(entity);

            return Result.Fail("Not Found", StatusCodes.Status404NotFound);
        }

        public async Task<T> GetByExpressionAsync(Expression<Func<T, bool>> expression, string includes = null, bool trackChanges = true)
        {
            IQueryable<T> query = _dbSet;
            var includesList = includes?.Split(", ");
            if (includesList is not null)
                query = includesList.Aggregate(query, (current, includesList) => current.Include(includesList));

            var item = trackChanges ? await query.FirstOrDefaultAsync(expression) : await query.AsNoTracking().FirstOrDefaultAsync(expression);

            return item;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression = null, string includes = null,
            bool trackChanges = true, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string distinctBy = null, int count = 0)
        {
            IQueryable<T> query = _dbSet;
            var includesList = includes?.Split(", ");
            if(includesList is not null)
                query = includesList.Aggregate(query, (current, includesList) => current.Include(includesList));   
            
            if(expression is not null)
                query = query.Where(expression);

            if (orderBy is not null)
                query = orderBy(query);

            if (count > 0)
                query = query.Take(count);

            var result = trackChanges ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();

            return result;
        }

        public Result<T> Update(T entity)
        {
            var result = _dbSet.Update(entity);
            return Result<T>.Succeed(entity);
        }
    }
}
