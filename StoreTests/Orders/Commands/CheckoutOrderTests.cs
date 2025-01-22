using Application.Common.Persistance;
using Application.Common.Services;
using Application.Orders.Commands;
using Application.Services;
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
    public class CheckoutOrderTests
    {
        private readonly Guid _userId;
        private readonly Mock<IGenericRepository<Order>> _orderRepoMock;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IGenericRepository<User>> _userRepoMock;
        private readonly Mock<IExchangeRate> _exchangeServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailSender> _mailSenderMock;
        private readonly Mock<IGenericRepository<Photo>> _photoRepoMock;

        public CheckoutOrderTests()
        {
            _userId = Guid.NewGuid();
            _orderRepoMock = new();
            _productRepoMock = new();
            _userRepoMock = new();
            _unitOfWorkMock = new();
            _exchangeServiceMock = new();
            _unitOfWorkMock = new();
            _mailSenderMock = new();
            _photoRepoMock = new();
        }


        [Fact]
        public async Task ChekoutOrderCommand_Should_Throw_Exception()
        {
            //Arrange

            var command = new CheckoutOrderCommand(_userId, false, "asd");

            _userRepoMock.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(new User
                {
                    EmailVerified = false
                });



            var handler = new CheckoutOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object, _exchangeServiceMock.Object,
                _mailSenderMock.Object, _photoRepoMock.Object);



            //Act
            //Assert

            await Assert.ThrowsAsync<EmailNotVerifiedException>(async () =>
            {
                await handler.Handle(command, default);
            });

        }


        [Fact]
        public async Task ChekoutOrderCommand_Should_Throw_UserIsBannedExcepion_When_User_Is_Banned()
        {
            var command = new CheckoutOrderCommand(_userId, false, "asd");

            _userRepoMock.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(new User
                {
                    EmailVerified = false,
                    Ban = true
                });

            var handler = new CheckoutOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object, _exchangeServiceMock.Object,
                _mailSenderMock.Object, _photoRepoMock.Object);

            await Assert.ThrowsAsync<UserIsBannedException>(async () =>
            {
                await handler.Handle(command, default);
            });
        }

        [Fact]
        public async Task ChekoutOrderCommand_Should_Throw_OrderNotFoundException()
        {
            var command = new CheckoutOrderCommand(_userId, false, "asd");

            _userRepoMock.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(new User
                {
                    EmailVerified = true,
                    Orders = new List<Order>()
                });



            var handler = new CheckoutOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object, _exchangeServiceMock.Object,
                _mailSenderMock.Object, _photoRepoMock.Object);


            await Assert.ThrowsAsync<OrderNotFoundException>(async () =>
            {
                await handler.Handle(command, default);
            });
        }


        [Theory]
        [InlineData(12, 4.5, 13)]
        [InlineData(100, 33, 130)]
        public async Task ChekoutOrderCommand_Should_Return_Success_False_When_Balance_Not_Enough(decimal price1, decimal price2, decimal balance)
        {
            //Arrange
            var command = new CheckoutOrderCommand(_userId, false, "asd");

            _userRepoMock.Setup(x => x.GetByExpressionAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new User
                {
                    Balance = balance,
                    EmailVerified = true,
                    Currency = "USD",
                    Orders = new List<Order>()
                        {
                            new Order
                            {
                                IsCompleted = false
                            }
                        }
                });

            _orderRepoMock.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order
                            {
                                IsCompleted = false,
                                Products = new List<Product>()
                                {
                                    new Product
                                    {
                                        Price = price1
                                    },
                                    new Product
                                    {
                                        Price = price2
                                    }
                                }
                            });

            _exchangeServiceMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRateResponse()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates() { USD = 1m, EUR = 2}
                });


            var handler = new CheckoutOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
                _userRepoMock.Object, _unitOfWorkMock.Object, _exchangeServiceMock.Object,
                _mailSenderMock.Object, _photoRepoMock.Object);



            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.False(actual.Success);
        }


        [Theory]
        [InlineData(12, 4.5, 20)]
        [InlineData(34, 16, 75)]
        public async Task ChekoutOrderCommand_Should_Checkout_Succesfuly(decimal price1, decimal price2, decimal balance)
        {
            var command = new CheckoutOrderCommand(_userId, false, "asd");

            _userRepoMock.Setup(x => x.GetByExpressionAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new User
                {
                    Balance = balance,
                    EmailVerified = true,
                    Currency = "USD",
                    Orders = new List<Order>()
                        {
                            new Order
                            {
                                IsCompleted = false
                            }
                        }
                });

            _orderRepoMock.Setup(x => x.GetByExpressionAsync(
                It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new Order
                {
                    IsCompleted = false,
                    Products = new List<Product>()
                                {
                                    new Product
                                    {
                                        Price = price1
                                    },
                                    new Product
                                    {
                                        Price = price2
                                    }
                                }
                });

            _exchangeServiceMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRateResponse()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates() { USD = 1m, EUR = 2 }
                });


            var handler = new CheckoutOrderCommandHander(_orderRepoMock.Object, _productRepoMock.Object,
              _userRepoMock.Object, _unitOfWorkMock.Object, _exchangeServiceMock.Object,
              _mailSenderMock.Object, _photoRepoMock.Object);



            //Act
            var actual = await handler.Handle(command, default);

            //Assert
            Assert.True(actual.Success);
            Assert.True(actual.Value.IsCompleted);

            

        }

    }
}
