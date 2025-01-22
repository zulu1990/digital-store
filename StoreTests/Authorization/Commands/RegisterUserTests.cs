using Application.Authorization.Commands;
using Application.Common.Handler;
using Application.Common.Persistance;
using Application.Common.Services;
using Domain;
using Domain.Entity;
using Infrastructure.Handler;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace StoreTests.Authorization.Commands
{
    public class RegisterUserTests
    {
        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly IPasswordHander _passwordHanderMock;
        private readonly Mock<IJwtTokenHander> _jwtTokenHanderMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;


        public RegisterUserTests()
        {
            _userRepositoryMock = new();
            _passwordHanderMock = new PasswordHandler();
            _jwtTokenHanderMock = new();
            _emailSenderMock = new();
            _unitOfWorkMock = new();

        }

        [Fact]
        public async Task RegisterUser_Fails_When_User_Already_Exist()
        {
            var command = new RegisterUserCommand("test@test.com", "password");

            var handler = new RegisterUserCommandHander(
                _userRepositoryMock.Object, _passwordHanderMock,
                _jwtTokenHanderMock.Object, _unitOfWorkMock.Object, _emailSenderMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), null, true))
                .ReturnsAsync(new User());


            var result = await handler.Handle(command, default);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.UserAlreadyExist, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_Success()
        {
            var email = "test@test.com";
            var command = new RegisterUserCommand(email, "password");

            var handler = new RegisterUserCommandHander(
                _userRepositoryMock.Object, _passwordHanderMock,
                _jwtTokenHanderMock.Object, _unitOfWorkMock.Object, _emailSenderMock.Object);


            var result = await handler.Handle(command, default);

            Assert.True(result.Success);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once());
            _emailSenderMock.Verify(x => x.SendMailVeryfication(email, It.IsAny<Guid>()), Times.Once());
            _unitOfWorkMock.Verify(x=> x.CommitAsync(), Times.Once());
            _jwtTokenHanderMock.Verify(x=> x.CreateToken(It.IsAny<User>()), Times.Once());
        }
    }
}
