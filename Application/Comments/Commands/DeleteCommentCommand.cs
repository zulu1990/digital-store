using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands
{
    public record DeleteCommentCommand(Guid UserId, Guid CommentId): IRequest<Result>;


    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result>
    {
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(IGenericRepository<Comment> commentRepository, IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            if(request.UserId == Guid.Empty)
                throw new UnauthorizedAccessException();

            var comment = await _commentRepository.GetByIdAsync(request.CommentId);
            if (comment is null)
                return Result.Fail(ErrorMessages.CommentNotFound, StatusCodes.Status404NotFound);


            if(comment.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            _commentRepository.DeleteEntitiy(comment);
            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
