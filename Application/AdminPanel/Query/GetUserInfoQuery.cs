using Application.AdminPanel.Models;
using Application.Common.Persistance;
using Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Query
{
    public record GetUserInfoQuery(Guid UserId): IRequest<GetUserInfoResponseModel>{}


    internal class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponseModel>
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Product> _productRepo;

        public GetUserInfoQueryHandler(IGenericRepository<User> userRepo, IGenericRepository<Order> orderRepo, IGenericRepository<Product> productRepo)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        public async Task<GetUserInfoResponseModel> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByExpressionAsync(x => x.Id == request.UserId, includes: "Orders", trackChanges: false);

            foreach (var order in user.Orders)
            {
                order.Products = await _productRepo.ListAsync(x => x.OrderId == order.Id);
            }

            var response = new GetUserInfoResponseModel
            {
                UserId = user.Id,
                Balance = user.Balance,
                Orders = user.Orders,
                Currency = user.Currency,
                Email = user.Email
            };

            return response;
        }
    }
}
