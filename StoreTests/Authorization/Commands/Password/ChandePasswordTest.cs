using Application.Authorization.Commands.Passwords;
using Application.Common.Handler;
using Application.Common.Persistance;
using Domain.Entity;
using Domain.Exceptions;
using Infrastructure.Handler;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Authorization.Commands.Password
{
    public class ChandePasswordTest
    {
        private readonly Mock<IGenericRepository<User>> _userRepoMock;
        private readonly IPasswordHander _passwordHandler;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly string _oldPasswordSample;
        private readonly string _newPasswordSample;
        private readonly Guid _userId;

        public ChandePasswordTest()
        {
            _passwordHandler = new PasswordHandler();
            _userRepoMock = new();
            _unitOfWorkMock = new();

            _userId = Guid.NewGuid();
            _oldPasswordSample = "Old Password";
            _newPasswordSample = "New Stronger Password";
        }

        [Fact]
        public async Task ChangePassword_Should_Throw_UserNotFoundException_When_Incorrect_UserId()
        {
            var command = new ChangePasswordCommand(_userId, _oldPasswordSample, _newPasswordSample);
            var handler = new ChangePasswordCommandHandler(_userRepoMock.Object,
                _passwordHandler, _unitOfWorkMock.Object);

            await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, default));
        }


        [Fact]
        public async Task ChangePassword_Should_Throw_IncorrectPasswordException_When_Wrong_Password()
        {
            //Arrange
            var command = new ChangePasswordCommand(_userId, _oldPasswordSample, _newPasswordSample);
            var handler = new ChangePasswordCommandHandler(_userRepoMock.Object,
                _passwordHandler, _unitOfWorkMock.Object);

            _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    PasswordHash = new byte[] {123, 11, 32},
                    PasswordSalt = new byte[] {0,1,2,3,4,5,6,7}
                });


            //Act
            //Assert
            await Assert.ThrowsAsync<IncorrectPasswordException>(() => handler.Handle(command, default));
        }


        [Fact]
        public async Task ChangePassword_Should_Return_Success_Result()
        {
            //Arrange

            var command = new ChangePasswordCommand(_userId, _oldPasswordSample, _newPasswordSample);
            var handler = new ChangePasswordCommandHandler(_userRepoMock.Object,
                _passwordHandler, _unitOfWorkMock.Object);

            _passwordHandler.CreateSaltAndHash(_oldPasswordSample, out var oldHash, out var oldSalt);

            _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    PasswordSalt = oldSalt,
                    PasswordHash= oldHash
                });

            //Act

            var result = await handler.Handle(command, default);

            //Assert
            Assert.True(result.Success);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);

        }
    }
}
