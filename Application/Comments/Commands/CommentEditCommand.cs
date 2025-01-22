using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Comments.Commands
{
    public record CommentEditCommand(Guid UserId, Guid CommentId, string? Message, int? Rating) : IRequest<Result<Comment>>;


    internal class CommentEditCommandHandler : IRequestHandler<CommentEditCommand, Result<Comment>>
    {
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CommentEditCommandHandler(IGenericRepository<Comment> commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Comment>> Handle(CommentEditCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Message) && request.Rating == null)
                return Result<Comment>.Fail(
                    ErrorMessages.IncorrectCommentParameters,
                    StatusCodes.Status403Forbidden);


            if (request.UserId == Guid.Empty)
                throw new UnauthorizedAccessException();

            var comment = await _commentRepository.GetByIdAsync(request.CommentId);

            if (comment is null)
                return Result<Comment>.Fail(ErrorMessages.CommentNotFound, StatusCodes.Status404NotFound);

            if (comment.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            comment.Message = request.Message;
            comment.Rating = request.Rating;

            _commentRepository.Update(comment);

            await _unitOfWork.CommitAsync();

            return Result<Comment>.Succeed(comment);
        }
    }
}
