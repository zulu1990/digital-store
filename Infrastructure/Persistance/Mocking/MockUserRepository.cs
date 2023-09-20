using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Mocking
{
    public class MockUserRepository: IGenericRepository<User>
    {
        private Dictionary<Guid, User> _dbSet;

        public MockUserRepository(MockDb db)
        {
            _dbSet = db.Users;
        }

        public async Task<User> AddAsync(User value)
        {
            await Task.CompletedTask;
            value.Id = Guid.NewGuid();
            _dbSet.TryAdd(value.Id, value);
            return value;
        }

        public async Task AddRangeAsync(IEnumerable<User> values)
        {
            foreach (var item in values)
            {
                await AddAsync(item);
            }
        }

        public Result DeleteEntitiy(User entity)
        {
            var result = DeleteEntityById(entity.Id).Result;
            return result;
        }

        public async Task<Result> DeleteEntityById(Guid id)
        {
            await Task.CompletedTask;
            if (_dbSet.Remove(id))
                return Result.Succeed();

            return Result.Fail("Couldn't Delete", StatusCodes.Status400BadRequest);
        }

        public async Task<User> GetByExpressionAsync(Expression<Func<User, bool>> expression, string includes = null, bool trackChanges = false)
        {
            await Task.CompletedTask;
            return _dbSet.Values.AsQueryable().Where(expression).FirstOrDefault();
        }

        public async Task<Result<User>> GetByIdAsync(Guid id)
        {
            await Task.CompletedTask;
            if (_dbSet.TryGetValue(id, out var result))
                return Result<User>.Succeed(result);

            return Result<User>.Fail("Not Found", StatusCodes.Status404NotFound);
        }

        public async Task<IList<User>> ListAsync(Expression<Func<User, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null, int count = 0)
        {
            await Task.CompletedTask;
            return _dbSet.Values.AsQueryable().Where(expression).ToList();
        }

        public Result<User> Update(User entity)
        {
            if (_dbSet.TryGetValue(entity.Id, out var result))
            {
                result.PasswordSalt = entity.PasswordSalt;
                result.PasswordHash = entity.PasswordHash;
                result.Email = entity.Email;
            }
            return Result<User>.Succeed(entity);
        }
    }
}
