using Application.Comments.Models.Output;
using Application.Common.Persistance;
using Application.Products.Models;
using Domain;
using Domain.Entity;
using Domain.Enum;
using MediatR;

namespace Application.Products.Query
{
    public record GetProductsByCategoryQuery(int ProductIdentifier, Guid UserId) : IRequest<Result<ProductDto>>;


    internal class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<ProductDto>>
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Photo> _photoRepo;
        private readonly IGenericRepository<Comment> _commentRepo;
        private readonly IGenericRepository<User> _userRepo;
        public GetProductsByCategoryQueryHandler(IGenericRepository<Product> productRepo,
            IGenericRepository<Photo> photoRepo, IGenericRepository<Comment> commentRepo, IGenericRepository<User> userRepo)
        {
            _productRepo = productRepo;
            _photoRepo = photoRepo;
            _commentRepo = commentRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByExpressionAsync(x=> x.ProductIdentifier == request.ProductIdentifier, trackChanges: false);

            var photo = await _photoRepo.GetByExpressionAsync(x => x.ProductIdentifier == request.ProductIdentifier, trackChanges: false);

            var comments = await _commentRepo.ListAsync(x => x.ProductIdentifier == product.ProductIdentifier, trackChanges: false);
            var commentsDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var commentDto = new CommentDto
                {
                    CommentId = comment.Id,
                    Message = comment.Message,
                    Rating = comment.Rating
                };
                if (comment.UserId != Guid.Empty)
                {
                    var user = await _userRepo.GetByExpressionAsync(x => x.Id == comment.UserId, trackChanges: false);
                    commentDto.Name = user.Email;
                }
                commentsDtos.Add(commentDto);
            }

            var averageRating = commentsDtos.Sum(x => x.Rating) / (double)comments.Count;

            var result = new ProductDto
            {
                ProductIdentifier = product.ProductIdentifier,
                Category = product.Category,
                Name = product.Name,
                Price = product.Price,
                Url = photo?.Url,
                Comments = commentsDtos,
                AverageRating = averageRating ??= 0
            };

            return Result<ProductDto>.Succeed(result);
        }
    }
}
