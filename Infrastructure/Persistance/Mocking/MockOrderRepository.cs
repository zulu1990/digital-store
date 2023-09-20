using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Infrastructure.Persistance.Mocking
{
    internal class MockOrderRepository : IGenericRepository<Order>
    {
        private readonly Dictionary<Guid, Order> _dbSet;

        public MockOrderRepository(MockDb mockDb)
        {
            _dbSet = mockDb.Orders;
        }

        public async Task<Order> AddAsync(Order value)
        {
            await Task.CompletedTask;
            value.Id = Guid.NewGuid();
            _dbSet.TryAdd(value.Id, value);
            return value;
        }

        public Task AddRangeAsync(IEnumerable<Order> values)
        {
            throw new NotImplementedException();
        }

        public Result DeleteEntitiy(Order entity)
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

        public Task<Order> GetByExpressionAsync(Expression<Func<Order, bool>> expression, string includes = null, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Order>> GetByIdAsync(Guid id)
        {
            await Task.CompletedTask;

            if(_dbSet.TryGetValue(id, out var value))
                return Result<Order>.Succeed(value);

            return Result<Order>.Fail("Not Found");
        }

        public Task<IList<Order>> ListAsync(Expression<Func<Order, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null, int count = 0)
        {
            throw new NotImplementedException();
        }

        public Result<Order> Update(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}
