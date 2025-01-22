using Application.Common.Persistance;
using Application.Products.Query;
using Domain.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Products.Query;

public class GetProductsByCategoryTests
{
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly Mock<IGenericRepository<Photo>> _photoRepoMock;
    private readonly Mock<IGenericRepository<Comment>> _commentRepoMock;
    private readonly Mock<IGenericRepository<User>> _userRepoMock;
    private readonly Guid _userId;

    public GetProductsByCategoryTests()
    {
        _commentRepoMock = new();
        _userRepoMock = new();
        _productRepoMock = new();
        _photoRepoMock = new();
        _userId = Guid.NewGuid();
    }

    [Fact]
    public async Task GetProductByCategory_Shoud_Return_ProductNotFound()
    {
        //Arrage
        var productIdentifier = 5;
        var errorMessage = "Product Not Found";
        var command = new GetProductsByCategoryQuery(productIdentifier, _userId);

        var handler = new GetProductsByCategoryQueryHandler(
            _productRepoMock.Object, _photoRepoMock.Object,
            _commentRepoMock.Object, _userRepoMock.Object);

        //Act
        var actual = await handler.Handle(command, default);

        //Assert
        Assert.False(actual.Success);
        Assert.Equal(404, actual.StatusCode);
        Assert.Equal(errorMessage, actual.Message);
    }

    [Fact]
    public async Task GetProductByCategory_Shoud_Return_Product_Without_Comments()
    {
        // Arrange
        var productIdentifier = 5;

        var command = new GetProductsByCategoryQuery(productIdentifier, _userId);

        var handler = new GetProductsByCategoryQueryHandler(
            _productRepoMock.Object, _photoRepoMock.Object,
            _commentRepoMock.Object, _userRepoMock.Object);

        _productRepoMock.Setup(x => 
            x.GetByExpressionAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>(), false))
            .ReturnsAsync(new Product
            {
                ProductIdentifier = productIdentifier,
                Name = "Test",
            });

        _commentRepoMock.Setup(x =>
            x.ListAsync(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), false, null, null, 0))
            .ReturnsAsync(new List<Comment>());


        //Act
        var result = await handler.Handle(command, default);

        //Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Empty(result.Value.Comments);
        Assert.Equal(productIdentifier, result.Value.ProductIdentifier);
        Assert.Equal(0, result.Value.AverageRating);
    }


    [Theory]
    [InlineData(4, 20)]
    [InlineData(1.4, 99)]
    [InlineData(0, 0)]
    public async Task GetProductByCategory_Shoud_Return_Product_With_Comment(int averageRating, int commentCount)
    {
        // Arrange
        var productIdentifier = 5;

        var query = new GetProductsByCategoryQuery(productIdentifier, _userId);

        var handler = new GetProductsByCategoryQueryHandler(
            _productRepoMock.Object, _photoRepoMock.Object,
            _commentRepoMock.Object, _userRepoMock.Object);

        _productRepoMock.Setup(x =>
            x.GetByExpressionAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>(), false))
            .ReturnsAsync(new Product
            {
                ProductIdentifier = productIdentifier,
                Name = "Test Product " + productIdentifier,
            });

        var comments = new List<Comment>();
        for (int i = 0; i < commentCount; i++)
        {
            comments.Add(new Comment
            {
                Id = Guid.NewGuid(),
                Message = $"Comment #{ i+1 } From {commentCount}",
                ProductIdentifier = productIdentifier,
                Rating = averageRating
            });
        }

        _commentRepoMock.Setup(x =>
            x.ListAsync(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<string>(), false, null, null, 0))
            .ReturnsAsync(comments);

        // Act
        var result = await handler.Handle(query, default);


        //Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(productIdentifier, result.Value.ProductIdentifier);
        Assert.Equal(averageRating, result.Value.AverageRating);
        Assert.Equal(commentCount, result.Value.Comments.Count);
    }


}
