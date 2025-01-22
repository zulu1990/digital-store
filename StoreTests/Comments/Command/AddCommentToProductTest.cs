using Application.Comments.Commands;
using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Comments.Command
{
    public class AddCommentToProductTest
    {
        private readonly Mock<IGenericRepository<Comment>> _commentRepoMock;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Guid _userId;

        public AddCommentToProductTest()
        {
            _userId = Guid.NewGuid();
            _commentRepoMock = new();
            _productRepoMock = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task AddCommentToProduct_Should_Throw_ProductNotFoundException_When_Incorrect_ProductIdentifier()
        {
            //Arrange
            var command = new AddCommentToProductCommand(_userId, "not empty", null, 1);

            var handler = new AddCommentToProductCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object,
                _productRepoMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<ProductNotFoundException>(
                async () => await handler.Handle(command, default));
        }

        [Fact]
        public async Task AddCommentToProduct_Should_Fail_When_Empty_Message_And_Rating()
        {
            //Arrange
            var command = new AddCommentToProductCommand(_userId, null, null, 1);

            var handler = new AddCommentToProductCommandHandler(
                _commentRepoMock.Object,
                _unitOfWorkMock.Object,
                _productRepoMock.Object);

            //Act

            var result = await handler.Handle(command, default);

            //Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.IncorrectCommentParameters, result.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        }

        [Theory]
        [InlineData("Great Product", null, 5)]
        [InlineData(null, 3, 2)]
        [InlineData("Awesome", 5, 1)]
        public async Task AddCommentToProduct_Should_Succees(string? message, int? rating, int productIdentifier)
        {
            //Arrange
            var command = new AddCommentToProductCommand(_userId, message, rating, productIdentifier);
            var handler = new AddCommentToProductCommandHandler(
            _commentRepoMock.Object, _unitOfWorkMock.Object, _productRepoMock.Object);

            _productRepoMock.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Product, bool>>>(), null, false))
                .ReturnsAsync(new Product());

            var expected = new Comment
            {
                Message = message,
                ProductIdentifier = productIdentifier,
                Rating = rating,
                UserId = _userId
            };

            //Act
            var result = await handler.Handle(command, default);

            //Assert

            Assert.True(result.Success);
            Assert.Equal(expected.Message, result.Value.Message);
            Assert.Equal(expected.Rating, result.Value.Rating);
            Assert.Equal(expected.ProductIdentifier, result.Value.ProductIdentifier);

            _unitOfWorkMock.Verify(x => x.CommitAsync(), times: Times.Once);
            _productRepoMock.Verify(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Product, bool>>>(), null, false), Times.Once);
            _commentRepoMock.Verify(x=> x.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

    }
}
