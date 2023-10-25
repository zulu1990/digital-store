using Application.Common.Persistance;
using Application.Products.Models;
using Application.Services;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;

namespace Application.Products.Query
{
    public record GetAllProductsQuery(Guid UserId) : IRequest<Result<IEnumerable<ProductDto>>>;


    internal class GetAllProductsQueryRequestHander : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductDto>>>
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Photo> _photoRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IExchangeRate _exchangeService;

        public GetAllProductsQueryRequestHander(IGenericRepository<Product> productRepo, IGenericRepository<Photo> photoRepo, IGenericRepository<User> userRepo, IExchangeRate exchangeService)
        {
            _productRepo = productRepo;
            _photoRepo = photoRepo;
            _userRepo = userRepo;
            _exchangeService = exchangeService;
        }

        public async Task<Result<IEnumerable<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {

            //TODO Add Functionality to get products by groupBy option

            var products = await _productRepo.ListAsync(trackChanges: false);

            var grouped = products.GroupBy(x => x.ProductIdentifier);
            var test = new List<Product>();

            foreach(var prod in grouped)
            {
                test.Add(prod.FirstOrDefault());
            }

            var result = test.Select(x => new ProductDto
            {
                ProductIdentifier = x.ProductIdentifier,
                Category = x.Category,
                Name = x.Name,
                Price = x.Price,
            }).ToList();


            foreach(var product in result)
            {
                var photo = await _photoRepo.GetByExpressionAsync(x => x.ProductIdentifier == product.ProductIdentifier, trackChanges:false);
                product.Url = photo?.Url;
            }



            if(request.UserId != Guid.Empty)
            {
                var user = await _userRepo.GetByIdAsync(request.UserId) 
                    ?? throw new UserNotFoundException("User Not Found");

                var rates = await _exchangeService.GetExchangeRates(user.Currency);

                result.ForEach(x => x.Price *= rates.conversion_rates.USD);
            }



            return Result<IEnumerable<ProductDto>>.Succeed(result);
        }
    }

}
