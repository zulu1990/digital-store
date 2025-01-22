using Application.Authorization.Commands.Passwords;
using Application.Common.Handler;
using Application.Common.Persistance;
using Application.Common.Services;
using Domain.Entity;
using Domain.Exceptions;
using Infrastructure.Handler;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.Authorization.Commands.Password
{
    public class ResetPasswordTest
    {

        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IPasswordHander _passwordHandler;
        private readonly string _email;
        private readonly string _password;

        public ResetPasswordTest()
        {
            _userRepositoryMock = new();
            _unitOfWorkMock = new();
            _passwordHandler = new PasswordHandler();
            _email = "test@test.com";
            _password = "new_password";
        }

        [Fact]
        public async Task ResetPassword_Throws_UserWasNotFoundException_When_Incorrect_Email()
        {
            //Arrange
            var command = new ResetPasswordCommand(_email, _password, Guid.NewGuid());

            var handler = new ResetPasswordCommandHandler(
                _userRepositoryMock.Object, _passwordHandler,
                _unitOfWorkMock.Object);


            await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task ResetPassword_Throws_InvalidTokenException_When_Incorrect_Token()
        {
            //Arrange
            var command = new ResetPasswordCommand(_email, _password, Guid.NewGuid());

            var handler = new ResetPasswordCommandHandler(
                _userRepositoryMock.Object, _passwordHandler,
                _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), null, true))
                .ReturnsAsync(new User() { Email = _email, VerificationCode = Guid.NewGuid() });


            await Assert.ThrowsAsync<InvalidTokenException>(()=> handler.Handle(command, default));

        }


        [Fact]
        public async Task ResetPassword_Should_Change_Password()
        {
            //Arrange
            var token = Guid.NewGuid();
            var command = new ResetPasswordCommand(_email, _password, token);

            var handler = new ResetPasswordCommandHandler(
                _userRepositoryMock.Object, _passwordHandler,
                _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), null, true))
                .ReturnsAsync(new User() { Email = _email, VerificationCode = token });


            //Act
            var result = await handler.Handle(command, default);


            //Assert
            Assert.True(result.Success);
            _unitOfWorkMock.Verify(x=> x.CommitAsync(), Times.Once());
        }


    }
}
