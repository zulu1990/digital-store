using Application.AdminPanel.Commands;
using Application.Common.Persistance;
using Application.Services;
using Domain;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Services.ImageUpload;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.AdminPanel
{
    public class AddProductTest
    {
        private readonly Mock<IGenericRepository<Product>> _productRepositoryMock;
        private readonly Mock<IGenericRepository<Photo>> _photoRepositoryMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddProductTest()
        {
            _productRepositoryMock = new();
            _photoRepositoryMock = new();
            _imageServiceMock = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task AddProduct_Should_Throw_Exception_When_No_Image()
        {
            //Arrange
            var command = new AddProductCommand(
                "Test Product", 11.1m, default, 
                ProductCategory.ESport, 1, 12);

            var handler = new AddProductCommandHander(_productRepositoryMock.Object,
                _unitOfWorkMock.Object, _photoRepositoryMock.Object, _imageServiceMock.Object);

            _imageServiceMock.Setup(x => x.UploadImage(It.IsAny<IFormFile>()))
                .ThrowsAsync(new FileNotFoundException());

            //Act
            //Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task AddProduct_Should_Fail_When_Price_Less_Then_Zero()
        {
            var command = new AddProductCommand(
                "Test Product", -11.1m, default,
                ProductCategory.ESport, 1, 12);

            var handler = new AddProductCommandHander(_productRepositoryMock.Object,
                _unitOfWorkMock.Object, _photoRepositoryMock.Object, _imageServiceMock.Object);
            
            //Act
            var result = await handler.Handle(command, default);


            //Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.NegativePriceDetected, result.Message);
            Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        }


        [Theory]
        [InlineData(123)]
        [InlineData(11)]
        [InlineData(1)]
        public async Task AddProduct_Should_Succeed(int count)
        {
            //Assert

            var command = new AddProductCommand(
                "Test Product", 11.1m, default,
                ProductCategory.ESport, 1, count);

            var handler = new AddProductCommandHander(_productRepositoryMock.Object,
                _unitOfWorkMock.Object, _photoRepositoryMock.Object, _imageServiceMock.Object);

            _imageServiceMock.Setup(x => x.UploadImage(It.IsAny<IFormFile>()))
                .ReturnsAsync(new Application.Images.Models.UploadImageResult());

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Exactly(count));
            _photoRepositoryMock.Verify(x=> x.AddAsync(It.IsAny<Photo>()), Times.Once());
            Assert.True(result.Success);
        }

    }
}
