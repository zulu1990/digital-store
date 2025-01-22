using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;

namespace Application.Orders.Commands
{
    public record AddProductToOrderCommand(Guid UserId, int ProductIdentifier, int Count) : IRequest<Result<Order>>;



    internal class AddProductToOrderCommandHander : IRequestHandler<AddProductToOrderCommand, Result<Order>>
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductToOrderCommandHander(IGenericRepository<Order> orderRepo, 
            IGenericRepository<Product> productRepo, IGenericRepository<User> userRepo, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<Order>> Handle(AddProductToOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId);

            if (user.Ban)
                throw new UserIsBannedException();

            var order = await _orderRepo.GetByExpressionAsync(x => x.UserId == request.UserId && x.IsCompleted == false, includes: "Products");
            var needsToAdd = order is null;

            var products = await _productRepo.ListAsync(
                p => p.ProductIdentifier == request.ProductIdentifier
                && p.OrderId == null && p.Sold == false
            , count: request.Count);


            order ??= new Order()
                {
                    UserId = request.UserId
                };


            foreach (var product in products)
            {
                product.OrderId = order.Id;
                order.Products.Add(product);
            }

           // user.Orders.Add(order);
           // _userRepo.Update(user);


            if(needsToAdd)
            {
                await _orderRepo.AddAsync(order);
            }
            else
            {
                _orderRepo.Update(order);
            }
            await _unitOfWork.CommitAsync();
            return Result<Order>.Succeed(order);
        }
    }

}
