using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Commands
{
    public record AddProductCommand(string Name, decimal Price,
        ProductCategory Category, int ProductIdentifier, int Count) : IRequest<Result>;



    internal class AddProductCommandHander : IRequestHandler<AddProductCommand, Result>
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductCommandHander(IGenericRepository<Product> productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            for(int i = 0; i< request.Count; i++)
            {
                var product = new Product
                {
                    Name = request.Name,
                    Price = request.Price,
                    Category = request.Category,
                    ProductIdentifier = request.ProductIdentifier,
                    OrderId = null,
                    Sold = false
                };

                await _productRepository.AddAsync(product);
            }

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
