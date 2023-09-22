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
    public class MockProductRepository: IGenericRepository<Product>
    {
        private readonly Dictionary<Guid, Product> _dbSet;
        public MockProductRepository(MockDb db)
        {
            _dbSet = db.Products;
        }
        public async Task<Product> AddAsync(Product value)
        {
            await Task.CompletedTask;
            value.Id = Guid.NewGuid();
            _dbSet.TryAdd(value.Id, value);
            return value;
        }

        public async Task AddRangeAsync(IEnumerable<Product> values)
        {
            foreach(var item in values)
            {
               await AddAsync(item);
            }
        }

        public Result DeleteEntitiy(Product entity)
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

        public async Task<Product> GetByExpressionAsync(Expression<Func<Product, bool>> expression, string includes = null, bool trackChanges = false)
        {
            await Task.CompletedTask;
           return _dbSet.Values.AsQueryable().Where(expression).FirstOrDefault();

        }

        public async Task<Result<Product>> GetByIdAsync(Guid id)
        {
            await Task.CompletedTask;

            if (_dbSet.TryGetValue(id, out var result))
                return Result<Product>.Succeed(result);

            return Result<Product>.Fail("Not Found", StatusCodes.Status404NotFound);
        }

        public async Task<IList<Product>> ListAsync(Expression<Func<Product, bool>> expression, string includes = null, bool trackChanges = false, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, int count = 0)
        {
            await Task.CompletedTask;
            if (count > 0)
            {
                return _dbSet.Values.AsQueryable().Where(expression).Take(count).ToList();
            }
            else
            {
                return _dbSet.Values.AsQueryable().Where(expression).ToList();
            }
        }

        public Result<Product> Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
