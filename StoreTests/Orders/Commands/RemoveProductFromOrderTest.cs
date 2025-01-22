using Application.Common.Persistance;
using Application.Orders.Commands;
using Domain.Entity;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Orders.Commands;

public class RemoveProductFromOrderTest
{
    private readonly Guid _userId;
    private readonly Mock<IGenericRepository<Order>> _orderRepoMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly Mock<IGenericRepository<User>> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private List<Product> _productsInOrder;


    public RemoveProductFromOrderTest()
    {
        _userId = Guid.NewGuid();
        _orderRepoMock = new();
        _productRepoMock = new();
        _userRepoMock = new();
        _unitOfWorkMock = new();

        FillProductsInOrder();


    }

    private void FillProductsInOrder()
    {
        _productsInOrder = new List<Product>();

        // productIndetifier = 1
        for (int i = 0; i < 20; i++)
        {
            _productsInOrder.Add(
                new Product
                {
                    ProductIdentifier = 1,
                    Name = "Product 1",
                    Id = Guid.NewGuid()
                }
              );
        }

        // productIndetifier = 2
        for (int i = 0; i < 10; i++)
        {
            _productsInOrder.Add(
                new Product
                {
                    ProductIdentifier = 2,
                    Name = "Product 2",
                    Id = Guid.NewGuid()
                }
              );
        }

        // productIndetifier = 5
        for (int i = 0; i < 15; i++)
        {
            _productsInOrder.Add(
                new Product
                {
                    ProductIdentifier = 5,
                    Name = "Product 5",
                    Id = Guid.NewGuid()
                }
              );
        }

    }

    [Fact]
    public async Task RemoveProductFromOrder_Should_Fail_When_User_Not_Exists()
    {
        //Arrange
        var count = 1;
        var productIdentifier = 5;

        var command = new RemoveProductFromOrderCommand(_userId, productIdentifier, count);

        var handler = new RemoveProductFromOrderCommandHander(
            _orderRepoMock.Object, _productRepoMock.Object,
            _userRepoMock.Object, _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<UserNotFoundException>(
            async () => await handler.Handle(command, default)
            );

    }



    [Fact]
    public async Task RemoveProductFromOrder_Should_Fail_When_Order_Not_Exists()
    {
        //Arrange
        var count = 1;
        var productIdentifier = 5;
        var errorMessage = "Ongoing Order Not Found";

        var command = new RemoveProductFromOrderCommand(_userId, productIdentifier, count);

        var handler = new RemoveProductFromOrderCommandHander(
            _orderRepoMock.Object, _productRepoMock.Object,
            _userRepoMock.Object, _unitOfWorkMock.Object);

        #region MOCK_OBJECTS

        _userRepoMock.Setup(x =>
        x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), "Orders", true))
            .ReturnsAsync(new User
            {
                Orders = new List<Order>()
            });

        #endregion




        //Act
        var result = await handler.Handle(command, default);

        //Assert
        Assert.False(result.Success);
        Assert.Equal(errorMessage, result.Message);
        Assert.Equal(404, result.StatusCode);

    }


    [Theory]
    [InlineData(5, 21)]
    [InlineData(2, 12)]
    [InlineData(1, 39)]
    public async Task RemoveProductFromOrder_Should_Fail_When_Not_Enough_Products(int productIdentifier, int count)
    {
        //Arrange
        var errorMessage = "Error due was not enough product in order";

        var command = new RemoveProductFromOrderCommand(_userId, productIdentifier, count);

        var handler = new RemoveProductFromOrderCommandHander(
            _orderRepoMock.Object, _productRepoMock.Object,
            _userRepoMock.Object, _unitOfWorkMock.Object);

        #region MOCK_OBJECTS

        _userRepoMock.Setup(x =>
        x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), "Orders", true))
            .ReturnsAsync(new User
            {
                Orders = new List<Order>()
                {
                    new Order
                    {
                        IsCompleted = false
                    }
                }
            });

        _orderRepoMock.Setup(x =>
        x.GetByExpressionAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Order
            {
                Products = _productsInOrder,

            });

        #endregion


        //Act
        var result = await handler.Handle(command, default);


        //Assert
        Assert.False(result.Success);
        Assert.Equal(errorMessage, result.Message);
    }



    [Theory]
    [InlineData(5, 15)]
    [InlineData(2, 6)]
    [InlineData(1, 7)]
    public async Task RemoveProductFromOrder_Should_Remove_Products(int productIdentifier, int count)
    {
        //Arrange
        var expected = _productsInOrder.Count - count;
        var command = new RemoveProductFromOrderCommand(_userId, productIdentifier, count);

        var handler = new RemoveProductFromOrderCommandHander(
            _orderRepoMock.Object, _productRepoMock.Object,
            _userRepoMock.Object, _unitOfWorkMock.Object);

        #region MOCK_OBJECTS

        _userRepoMock.Setup(x =>
        x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), "Orders", true))
            .ReturnsAsync(new User
            {
                Orders = new List<Order>()
                {
                    new Order
                    {
                        IsCompleted = false,
                        Products = _productsInOrder
                    }
                }
            });

        _orderRepoMock.Setup(x =>
        x.GetByExpressionAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Order
            {
                Products = _productsInOrder,

            });

        #endregion


        //Act
        var result = await handler.Handle(command, default);

        

        //Assert
        Assert.True(result.Success);
        Assert.Equal(expected, result.Value.Products.Count);
    }



}
