using Application.Authorization.Queries;
using Application.Common.Handler;
using Application.Common.Persistance;
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

namespace StoreTests.Authorization.Query
{
    public class LoginTest
    {
        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly Mock<IPasswordHander> _passwordHanderMock;
        private readonly Mock<IJwtTokenHander> _jwtTokenHanderMock;
        private readonly string _email;
        private readonly string _password;

        public LoginTest()
        {
            _jwtTokenHanderMock = new Mock<IJwtTokenHander>();
            _passwordHanderMock = new Mock<IPasswordHander>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();

            _password = "password 123";
            _email = "test@test.com";
        }

        [Fact]
        public async Task LoginQuery_Should_Fail_When_Incorrect_Email()
        {
            var loginQuery= new LoginQuery(_email, _password);
            var handler = new LoginQueryHandler(_userRepositoryMock.Object,
                _passwordHanderMock.Object, _jwtTokenHanderMock.Object);

            var result = await handler.Handle(loginQuery, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.IncorrectCredentials, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        }

        [Fact]
        public async Task LoginQuery_Should_Fail_When_Incorrect_Email_And_Password()
        {

            var loginQuery = new LoginQuery(_email, _password);
            var handler = new LoginQueryHandler(_userRepositoryMock.Object,
                _passwordHanderMock.Object, _jwtTokenHanderMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), null, false))
                .ReturnsAsync(new User());

            _passwordHanderMock.Setup(
                x => x.ValidatePassword(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(false);

            var result = await handler.Handle(loginQuery, default);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.IncorrectCredentials, result.Message);
        }

        [Fact]
        public async Task LoginQuery_Should_Succeed()
        {
            var loginQuery = new LoginQuery(_email, _password);
            var handler = new LoginQueryHandler(_userRepositoryMock.Object,
                _passwordHanderMock.Object, _jwtTokenHanderMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(
                    It.IsAny<Expression<Func<User, bool>>>(), null, false))
                .ReturnsAsync(new User());

            _passwordHanderMock.Setup(
                x => x.ValidatePassword(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(true);

            var result = await handler.Handle(loginQuery, default);

            Assert.True(result.Success);
            _jwtTokenHanderMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Once());
        }



    }
}
