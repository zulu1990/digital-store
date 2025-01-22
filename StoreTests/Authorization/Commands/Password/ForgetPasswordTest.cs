using Application.Authorization.Commands.Passwords;
using Application.Common.Persistance;
using Application.Common.Services;
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

namespace StoreTests.Authorization.Commands.Password
{
    public class ForgetPasswordTest
    {
        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly string _email;

        public ForgetPasswordTest()
        {
            _userRepositoryMock = new();
            _emailSenderMock = new();
            _unitOfWorkMock = new();
            _email = "test@test.com";
        }


        [Fact]
        public async Task ForgetPassword_Shoud_Throw_UserNotFoundException_When_Incorrect_Email()
        {
            //Arrange
            var command = new ForgotPasswordCommand(_email);
            var handler = new ForgotPasswordCommandHander(
                _userRepositoryMock.Object, _emailSenderMock.Object,
                _unitOfWorkMock.Object);


            await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, default));
        }


        [Fact]
        public async Task ForgetPassword_Should_Return_Success_Status()
        {
            //Arrange
            var command = new ForgotPasswordCommand(_email);
            var handler = new ForgotPasswordCommandHander(
                _userRepositoryMock.Object, _emailSenderMock.Object,
                _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), null, true))
                .ReturnsAsync(new User() { Email = _email });


            //Act
            var result = await handler.Handle(command, default);

            //Assert
            _emailSenderMock.Verify(x => x.SendPasswordResetMail(_email, It.IsAny<Guid>()), Times.Once());
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once());
            Assert.True(result.Success);
        }

    }
}
