using Application.Common.Persistance;
using Application.Customer.Models;
using Application.Orders.Models;
using Application.Products.Models;
using Application.Services;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using System.Collections.Generic;

namespace Application.Customer.Queries
{
    public record UserProfileQuery(Guid UserId) : IRequest<Result<UserProfileResponse>>;


    internal class UserProfileQueryHandler : IRequestHandler<UserProfileQuery, Result<UserProfileResponse>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IExchangeRate _exchangeRate;

        public UserProfileQueryHandler(IGenericRepository<User> userRepository,
            IGenericRepository<Product> productRepository, IExchangeRate exchangeRate)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _exchangeRate = exchangeRate;
        }

        public async Task<Result<UserProfileResponse>> Handle(UserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x => x.Id == request.UserId, includes: "Orders");

            if (user == null)
                throw new UserNotFoundException("User Not Found");

            var ordersRepsonse = new List<OrderResponse>();

            foreach(var order in user.Orders)
            {
                var products = await _productRepository.ListAsync(x => x.OrderId == order.Id, trackChanges: false);
                
                ordersRepsonse.Add(new OrderResponse
                {
                    EndDate = order.EndDate,
                    IsCompleted = order.IsCompleted,
                    StartDate = order.StartDate,
                    Products = products.Select(x => new ProductDto
                    {
                        Category = x.Category,
                        Name = x.Name,
                        Price = x.Price,
                        ProductIdentifier = x.ProductIdentifier,
                    }).ToList()
                });
            }

            for (int i = 0; i< user.Orders.Count; i++)
            {
                var order = user.Orders.ToList()[i];
                var products = await _productRepository.ListAsync(x => x.OrderId == order.Id, trackChanges: false);
                order.Products.ToList().AddRange(products);
            }

            var exchangeRate = await _exchangeRate.GetExchangeRates(user.Currency);

            var response = new UserProfileResponse()
            {
                Balance = user.Balance,
                Currency = user.Currency,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                ExchangeRates = exchangeRate.conversion_rates,
                Orders = ordersRepsonse,
            };

            return Result<UserProfileResponse>.Succeed(response);


        }
    }
}
