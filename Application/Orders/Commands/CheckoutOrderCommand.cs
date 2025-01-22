using Application.Common.Persistance;
using Application.Common.Services;
using Application.Services;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Orders.Commands
{
    public record CheckoutOrderCommand (Guid UserId, bool Delivery, string? Address): IRequest<Result<Order>>;
    internal class CheckoutOrderCommandHander : IRequestHandler<CheckoutOrderCommand, Result<Order>>
    {
        private readonly IGenericRepository<Order> _ordersRepo;
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<User> _usersRepo;
        private readonly IGenericRepository<Photo> _photoRepo;

        private readonly IExchangeRate _exchangeService;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;


        public CheckoutOrderCommandHander(IGenericRepository<Order> ordersRepo, IGenericRepository<Product> productsRepo,
            IGenericRepository<User> usersRepo, IUnitOfWork unitOfWork, IExchangeRate exchangeService, IEmailSender emailSender, IGenericRepository<Photo> photoRepo)
        {
            _ordersRepo = ordersRepo;
            _productsRepo = productsRepo;
            _usersRepo = usersRepo;
            _unitOfWork = unitOfWork;
            _exchangeService = exchangeService;
            _emailSender = emailSender;
            _photoRepo = photoRepo;
        }

        public async Task<Result<Order>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRepo.GetByExpressionAsync(x=> x.Id == request.UserId, includes: "Orders");

            if (user.Ban)
                throw new UserIsBannedException();

            if (user.EmailVerified == false)
                throw new EmailNotVerifiedException("Please verify email before checkout");

            var ongoingOrder = user.Orders.FirstOrDefault(x => x.IsCompleted == false);

            if(ongoingOrder == null)
            {
                throw new OrderNotFoundException("There was not ongoing order", user.Id);
            }

            var orderFromDb = await _ordersRepo.GetByExpressionAsync(x => x.Id == ongoingOrder.Id, includes: "Products");


            var totalSum = orderFromDb.Products.Sum(x => x.Price);
            var photos = new List<Photo>();

            foreach(var product in orderFromDb.Products)
            {
                photos.Add(await _photoRepo.GetByExpressionAsync(x => x.ProductIdentifier == product.ProductIdentifier));
            }

            var rates = await _exchangeService.GetExchangeRates(user.Currency);

            var totalSumConverted = totalSum * rates.conversion_rates.USD;

            if( user.Balance < totalSumConverted)
                return Result<Order>.Fail("NotEnoughBalance");


            user.Balance -= totalSum;

            orderFromDb.Products.ToList().ForEach(x => x.Sold = true);
            orderFromDb.IsCompleted = true;
            orderFromDb.EndDate = DateTime.UtcNow;

            var requestDelivery = string.IsNullOrWhiteSpace(request.Address)? user.Address : request.Address;

            if (request.Delivery)
            {
                await _emailSender.SendEmailAsync(user.Email, "Order Finished", $"your order will be sent to: {requestDelivery}" +
                    $"<img src={photos.FirstOrDefault().Url} alt=\"Italian Trulli\">", true);
            }

            await _unitOfWork.CommitAsync();

            return Result<Order>.Succeed(orderFromDb);
        }
    }
}
