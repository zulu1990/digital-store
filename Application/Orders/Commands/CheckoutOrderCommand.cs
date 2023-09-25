﻿using Application.Common.Persistance;
using Application.Services;
using Domain;
using Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Orders.Commands
{
    public record CheckoutOrderCommand (Guid UserId): IRequest<Result<Order>>;
    internal class CheckoutOrderCommandHander : IRequestHandler<CheckoutOrderCommand, Result<Order>>
    {
        private readonly IGenericRepository<Order> _ordersRepo;
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<User> _usersRepo;
        private readonly IExchangeRate _exchangeService;

        private readonly IUnitOfWork _unitOfWork;


        public CheckoutOrderCommandHander(IGenericRepository<Order> ordersRepo, IGenericRepository<Product> productsRepo, 
            IGenericRepository<User> usersRepo, IUnitOfWork unitOfWork, IExchangeRate exchangeService)
        {
            _ordersRepo = ordersRepo;
            _productsRepo = productsRepo;
            _usersRepo = usersRepo;
            _unitOfWork = unitOfWork;
            _exchangeService = exchangeService;
        }

        public async Task<Result<Order>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRepo.GetByExpressionAsync(x=> x.Id == request.UserId, includes: "Orders");

            var ongoingOrder = user.Orders.FirstOrDefault(x => x.IsCompleted == false);

            //TODO
            // check ongoingOrder exists


            var totalSum = ongoingOrder.Products.Sum(x => x.Price);
            var rates = await _exchangeService.GetExchangeRates(user.Currency);
            var totalSumConverted = totalSum * rates.conversion_rates.USD;

            if( user.Balance < totalSumConverted)
                return Result<Order>.Fail("NotEnoughBalance");


            user.Balance -= totalSum;

            ongoingOrder.Products.ToList().ForEach(x => x.Sold = true);
            ongoingOrder.IsCompleted = true;

            await _unitOfWork.CommitAsync();

            return Result<Order>.Succeed(ongoingOrder);
        }
    }
}
