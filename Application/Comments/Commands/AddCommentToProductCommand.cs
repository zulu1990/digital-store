using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Comments.Commands
{
    public record AddCommentToProductCommand(
        Guid UserId, string? Message, 
        int? Rating, int ProductIdentifier) : IRequest<Result<Comment>>;

    public class AddCommentToProductCommandHandler : IRequestHandler<AddCommentToProductCommand, Result<Comment>>
    {
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddCommentToProductCommandHandler(IGenericRepository<Comment> commentRepository, 
            IUnitOfWork unitOfWork, IGenericRepository<Product> productRepository)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public async Task<Result<Comment>> Handle(AddCommentToProductCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Message) && request.Rating == null)
                return Result<Comment>.Fail(
                    ErrorMessages.IncorrectCommentParameters, 
                    StatusCodes.Status403Forbidden);


            var product = await _productRepository.GetByExpressionAsync(x => x.ProductIdentifier == request.ProductIdentifier, trackChanges: false);

            if (product is null)
                throw new ProductNotFoundException(request.ProductIdentifier);

            var comment = new Comment()
            {
                Message = request.Message,
                ProductIdentifier = request.ProductIdentifier,
                Rating = request.Rating,
                UserId = request.UserId
            };

            await _commentRepository.AddAsync(comment);

            await _unitOfWork.CommitAsync();

            return Result<Comment>.Succeed(comment);
        }
    }


}
