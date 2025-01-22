using Application.Comments.Commands;
using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Comments.Command
{
    public class DeleteCommentTest
    {
        private readonly Mock<IGenericRepository<Comment>> _commentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Guid _userId;
        private readonly Guid _commentId;

        public DeleteCommentTest()
        {
            _commentRepositoryMock = new();
            _unitOfWorkMock = new();
            _userId = Guid.NewGuid();
            _commentId = Guid.NewGuid();
        }

        [Fact]
        public async Task DeleteComment_Should_Throw_Unauthorized_Exception_When_UserId_Equals_Guid_Empty()
        {
            //Arrange
            var userId = Guid.Empty;
            var command = new DeleteCommentCommand(userId, _commentId);

            var handler = new DeleteCommentCommandHandler(
                _commentRepositoryMock.Object, 
                _unitOfWorkMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task DeleteComment_Should_Return_Failure_When_Comment_Not_Exist()
        {
            //Arrange
            var command = new DeleteCommentCommand(_userId, _commentId);

            var handler = new DeleteCommentCommandHandler(
                _commentRepositoryMock.Object,
                _unitOfWorkMock.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.CommentNotFound, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        }

        [Fact]
        public async Task DeleteComment_Should_Throw_Unauthorized_Exception_When_UserId_Not_Equal_Comment_UserId()
        {
            //Arrange
            var command = new DeleteCommentCommand(_userId, _commentId);

            var handler = new DeleteCommentCommandHandler(
                _commentRepositoryMock.Object,
                _unitOfWorkMock.Object);

            _commentRepositoryMock.Setup(x=> 
                x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Comment 
                { 
                    Id = _commentId, 
                    UserId = Guid.NewGuid() 
                });


            //Act
            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task DeleteComment_Should_Success()
        {
            //Arrange
            var command = new DeleteCommentCommand(_userId, _commentId);

            var handler = new DeleteCommentCommandHandler(
                _commentRepositoryMock.Object,
                _unitOfWorkMock.Object);

            _commentRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Comment
                {
                    Id = _commentId,
                    UserId = _userId
                });

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }

    }
}
