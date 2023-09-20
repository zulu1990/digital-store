using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Enum;
using MediatR;

namespace Application.Products.Query
{
    public record GetProductsByCategoryQuery(ProductCategory Category) : IRequest<Result<IEnumerable<Product>>>;


    internal class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<IEnumerable<Product>>>
    {
        private readonly IGenericRepository<Product> _repository;
        public GetProductsByCategoryQueryHandler(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<Product>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var results = await _repository.ListAsync(x=> x.Category == request.Category);

            return Result<IEnumerable<Product>>.Succeed(results);
        }
    }
}
