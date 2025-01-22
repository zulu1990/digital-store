using Application.Common.Persistance;
using Application.Services;
using Domain;
using Domain.Entity;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.AdminPanel.Commands
{
    public record AddProductCommand(string Name, decimal Price, IFormFile Image,
        ProductCategory Category, int ProductIdentifier, int Count) : IRequest<Result>;



    internal class AddProductCommandHander : IRequestHandler<AddProductCommand, Result>
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Photo> _photoRepository;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public AddProductCommandHander(IGenericRepository<Product> productRepository, IUnitOfWork unitOfWork,
            IGenericRepository<Photo> photoRepository, IImageService imageService)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _photoRepository = photoRepository;
            _imageService = imageService;
        }

        public async Task<Result> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            if (request.Price < 0)
                return Result.Fail(ErrorMessages.NegativePriceDetected, StatusCodes.Status403Forbidden);


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

            var photoUpload = await _imageService.UploadImage(request.Image);

            var photo = new Photo
            {
                ProductIdentifier = request.ProductIdentifier,
                PublicId = photoUpload.PublicId,
                Url = photoUpload.Url,
            };

            await _photoRepository.AddAsync(photo);
            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
