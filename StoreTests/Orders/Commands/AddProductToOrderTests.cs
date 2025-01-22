using Application.Common.Persistance;
using Application.Orders.Commands;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Orders.Commands
{
    public class AddProductToOrderTests
    {
        private readonly Guid _userId;
        private readonly Mock<IGenericRepository<Order>> _orderRepoMock;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IGenericRepository<User>> _userRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;


        public AddProductToOrderTests()
        {
            _userId = Guid.NewGuid();
            _orderRepoMock = new();
            _productRepoMock = new();
            _userRepoMock = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task AddPoductToOrder_Should_Return_New_Order()
        {
            //Arrange
            var count = 2;
            var productIdentifier = 1;

            var products = new List<Product>()
            {
                new Product {Name = "Product 1", ProductIdentifier = productIdentifier, Sold = false,  OrderId = null},
                new Product {Name = "Product 1", ProductIdentifier = productIdentifier, Sold = false,  OrderId = null}
            };

            _productRepoMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>(), 
                It.IsAny<bool>(), null, null, count)).ReturnsAsync(products);

            var command = new AddProductToOrderCommand(_userId, productIdentifier, count);


            var handler = new AddProductToOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object);

            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.True(actual.Success);
            Assert.Equal(actual.Value.Products.Count, count);
            Assert.False(actual.Value.IsCompleted);
        }



        [Fact]
        public async Task AddPoductToOrder_Should_Update_Order()
        {
            var productIdentifier = 1;
            var count = 2;

            var products = new List<Product>()
            {
                new Product {Name = "Product 1", ProductIdentifier = productIdentifier, Sold = false,  OrderId = null},
                new Product {Name = "Product 1", ProductIdentifier = productIdentifier, Sold = false,  OrderId = null}
            };

            _productRepoMock.Setup(x => x.ListAsync(
                It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>(),
                It.IsAny<bool>(), null, null, count)).ReturnsAsync(products);


            _orderRepoMock.Setup(x=> x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), true))
                .ReturnsAsync(new Order
                {
                    IsCompleted = false,
                    Products = new List<Product>()
                    {
                        new Product
                        {
                            Name = "Existing one"
                        }
                    }
                });

            var command = new AddProductToOrderCommand(_userId, productIdentifier, count);


            var handler = new AddProductToOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object);

            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.True(actual.Success);
            Assert.Equal(actual.Value.Products.Count, count + 1);
            Assert.False(actual.Value.IsCompleted);

        }

        [Fact]
        public async Task AddPRoductToOrder_Should_Throw_UserIsBannedExcepion_When_User_Is_Banned()
        {
            //Arrange
            var count = 2;
            var productIdentifier = 1;
            _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Ban = true
                });

            var command = new AddProductToOrderCommand(_userId, productIdentifier, count);


            var handler = new AddProductToOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object);

            //Act
            //Assert
            await Assert.ThrowsAsync<UserIsBannedException>(() => handler.Handle(command, default));

        }
    }
}
