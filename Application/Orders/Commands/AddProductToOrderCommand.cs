using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;

namespace Application.Orders.Commands
{
    public record AddProductToOrderCommand(Guid UserId, Guid ProductId, int Count) : IRequest<Result<Order>>;



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

            //TODO
            //check if user exists

            var order = await _orderRepo.GetByExpressionAsync(x => x.UserId == request.UserId, includes: "Products");
            var productResult = await _productRepo.GetByIdAsync(request.ProductId);

            var product = productResult.Value;

            if (order is null)
            {
                order = new Order()
                {
                    UserId = request.UserId
                };

                order.Products.Add(product);
            }
            else
            {
                var existingProduct = order.Products.FirstOrDefault(x => x.Id == request.ProductId);

                if (existingProduct != null)
                {
                    existingProduct.ReminingCount += request.Count;
                }
                else
                {
                    order.Products.Add(product);
                }
            }

            return Result<Order>.Succeed(order);
        }
    }

}
