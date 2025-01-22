using Domain.Entity;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Persistance;
using Domain.Exceptions;

namespace Application.Orders.Commands
{
    public record RemoveProductFromOrderCommand(Guid UserId, int ProductIdentifier, int Count) : IRequest<Result<Order>>;


    internal class RemoveProductFromOrderCommandHander : IRequestHandler<RemoveProductFromOrderCommand, Result<Order>>
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveProductFromOrderCommandHander(IGenericRepository<Order> orderRepo,
            IGenericRepository<Product> productRepo, IGenericRepository<User> userRepo, IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<Order>> Handle(RemoveProductFromOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByExpressionAsync(x=> x.Id == request.UserId, includes: "Orders");

            if (user is null)
                throw new UserNotFoundException("User Not Found");


            var ongoingOrder = user.Orders.FirstOrDefault(o => o.IsCompleted == false);

            if (ongoingOrder == null)
                return Result<Order>.Fail("Ongoing Order Not Found", 404);


            var orderFromDb = await _orderRepo.GetByExpressionAsync(x => x.Id == ongoingOrder.Id, includes: "Products");
            
            var productsFromOrder = orderFromDb.Products.Where(x => x.ProductIdentifier == request.ProductIdentifier).ToList();
            
            if(productsFromOrder.Count() < request.Count)
                return Result<Order>.Fail("Error due was not enough product in order");

            var productsToRemove = productsFromOrder.Take(request.Count).ToList();

            for (int i = 0; i < productsToRemove.Count; i++)
            {
                ongoingOrder.Products.Remove(productsToRemove[i]);
                productsToRemove[i].OrderId = null;
            }
            await _unitOfWork.CommitAsync();

            return Result<Order>.Succeed(ongoingOrder);
        }
    }
}
