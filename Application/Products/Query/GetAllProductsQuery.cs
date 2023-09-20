using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;

namespace Application.Products.Query
{
    public record GetAllProductsQuery : IRequest<Result<IEnumerable<Product>>>;


    internal class GetAllProductsQueryRequestHander : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<Product>>>
    {
        private readonly IGenericRepository<Product> _productRepo;
        public GetAllProductsQueryRequestHander(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<Result<IEnumerable<Product>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepo.ListAsync(x => true);

            return Result<IEnumerable<Product>>.Succeed(products);
        }
    }

}
