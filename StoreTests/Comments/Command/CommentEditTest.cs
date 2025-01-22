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
    public class CommentEditTest
    {
        private readonly Mock<IGenericRepository<Comment>> _commentRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Guid _userId;
        private readonly Guid _commentId;

        public CommentEditTest()
        {
            _commentRepoMock = new();
            _commentId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _unitOfWorkMock = new();
        }


        [Fact]
        public async Task CommentEdit_Should_Fail_When_Empty_Message_And_Rating()
        {
            //Arrange
            var command = new CommentEditCommand(_userId, _commentId, null, null);

            var handler = new CommentEditCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object);

            //Act

            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.IncorrectCommentParameters, result.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task CommentEdit_Should_Throw_UnauthorizedAccessException_When_UserID_Empty()
        {
            //Arrange
            var command = new CommentEditCommand(Guid.Empty, _commentId, "Not null", null);

            var handler = new CommentEditCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object);


            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task CommentEdit_Should_Fail_When_Comment_Not_Found()
        {
            //Arrange
            var command = new CommentEditCommand(_userId, _commentId, "Not null", null);

            var handler = new CommentEditCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object);

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ErrorMessages.CommentNotFound, result.Message);
        }

        [Fact]
        public async Task CommentEdit_Should_Throw__UnauthorizedAccessException_When_Requester_UserID_Not_Equal_Saved()
        {
            //Arrange
            var command = new CommentEditCommand(_userId, _commentId, "Not null", null);

            var handler = new CommentEditCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object);

            _commentRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Comment() { UserId = Guid.NewGuid() });


            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, default));
        }


        [Theory]
        [InlineData("Great Product", null)]
        [InlineData(null, 3)]
        [InlineData("Awesome", 5)]
        public async Task AddCommentToProduct_Should_Succees(string? message, int? rating)
        {
            //Arrange
            var command = new CommentEditCommand(_userId, _commentId, message, rating);

            var handler = new CommentEditCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object);

            _commentRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Comment
                {
                    UserId = _userId,
                    Id = _commentId,
                    Rating = - 123,
                    Message = "Old Message, Will be Replaced"
                });



            //Act
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.Success);
            Assert.Equal(message, result.Value.Message);
            Assert.Equal(rating, result.Value.Rating);

            _unitOfWorkMock.Verify(x => x.CommitAsync(), times: Times.Once);
            _commentRepoMock.Verify(x => x.Update(It.IsAny<Comment>()), Times.Once);
        }






    }
}
