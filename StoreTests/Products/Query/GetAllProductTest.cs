using Application.Common.Persistance;
using Application.Products.Query;
using Application.Services;
using Domain.Entity;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Products.Query
{
    public class GetAllProductTest
    {
        private readonly Guid _userId;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IGenericRepository<Photo>> _photoRepoMock;
        private readonly Mock<IGenericRepository<User>> _userRepoMock;
        private readonly Mock<IExchangeRate> _exchangeServiceMock;

        public GetAllProductTest()
        {
            _userId = Guid.NewGuid();
            _productRepoMock = new();
            _photoRepoMock = new();
            _userRepoMock = new();
            _exchangeServiceMock = new();
        }


        [Fact]
        public async Task GetAllProductQuery_Should_Return_One_Product()
        {
            //Arrange
            var products = new List<Product>()
            {
                new Product()
                {
                    Category = Domain.Enum.ProductCategory.Sport,
                    Name = "Product 1",
                    ProductIdentifier = 1,
                    Price = 1,
                    Sold = false,
                    OrderId = null
                },
                new Product()
                {
                    Category = Domain.Enum.ProductCategory.Sport,
                    Name = "Product 1",
                    ProductIdentifier = 1,
                    Price = 1,
                    Sold = false,
                    OrderId = null
                }
            };

            var query = new GetAllProductsQuery(_userId);


            _productRepoMock.Setup(x => x.ListAsync(null, null, false, null, null, 0))
                .ReturnsAsync(products);

            _exchangeServiceMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRateResponse()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates()
                });

            _userRepoMock.Setup(x => x.GetByIdAsync(_userId))
                .ReturnsAsync(new User()
                {
                    Currency = "USD"
                });


            var handler = new GetAllProductsQueryRequestHander(_productRepoMock.Object, _photoRepoMock.Object,
                           _userRepoMock.Object, _exchangeServiceMock.Object);


            //Act
            var result = await handler.Handle(query, default);

            //Assert
            Assert.True(result.Success);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetAllProductQuery_Should_Return_Empty_List()
        {
            //Arrange
            _productRepoMock.Setup(x => x.ListAsync(null, null, false, null, null, 0))
                .ReturnsAsync(new List<Product>());


            var query = new GetAllProductsQuery(Guid.Empty);

            var handler = new GetAllProductsQueryRequestHander(_productRepoMock.Object, _photoRepoMock.Object,
                _userRepoMock.Object, _exchangeServiceMock.Object);



            //Act
            var result = await handler.Handle(query, default);

            //Assert

            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllProductsQuery_Should_Throw_UserNotFoundExcpetion()
        {
            //Arrange
            _productRepoMock.Setup(x => x.ListAsync(null, null, false, null, null, 0))
                .ReturnsAsync(new List<Product>());


            var query = new GetAllProductsQuery(_userId);

            var handler = new GetAllProductsQueryRequestHander(_productRepoMock.Object, _photoRepoMock.Object,
                _userRepoMock.Object, _exchangeServiceMock.Object);



            //Act
            await Assert.ThrowsAsync<UserNotFoundException>(async () =>
            {
                await handler.Handle(query, default);
            });
        }

        [Fact]
        public async Task GetAllProductQuery_Should_Return_Two_Products()
        {
            //Arrange
            var products = new List<Product>()
            {
                new Product()
                {
                    Category = Domain.Enum.ProductCategory.Sport,
                    Name = "Product 1",
                    ProductIdentifier = 1,
                    Price = 1,
                    Sold = false,
                    OrderId = null
                },
                new Product()
                {
                    Category = Domain.Enum.ProductCategory.Sport,
                    Name = "Product 2",
                    ProductIdentifier = 2,
                    Price = 1,
                    Sold = false,
                    OrderId = null
                }
            };

            var query = new GetAllProductsQuery(_userId);


            _productRepoMock.Setup(x => x.ListAsync(null, null, false, null, null, 0))
                .ReturnsAsync(products);

            _exchangeServiceMock.Setup(x => x.GetExchangeRates("USD"))
                .ReturnsAsync(new Application.Services.Models.ExchangeRateResponse()
                {
                    conversion_rates = new Application.Services.Models.Conversion_Rates()
                });

            _userRepoMock.Setup(x => x.GetByIdAsync(_userId))
                .ReturnsAsync(new User()
                {
                    Currency = "USD"
                });


            var handler = new GetAllProductsQueryRequestHander(_productRepoMock.Object, _photoRepoMock.Object,
                           _userRepoMock.Object, _exchangeServiceMock.Object);


            //Act
            var result = await handler.Handle(query, default);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Value.Count());
        }


    }
}
