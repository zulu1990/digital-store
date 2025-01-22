using Application.Authorization.Commands;
using Application.Common.Persistance;
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

namespace StoreTests.Authorization.Commands
{
    public class VerifyUserEmailTest
    {
        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;


        public VerifyUserEmailTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IGenericRepository<User>>();
        }

        [Fact]
        public async Task VerifyUserEmail_Should_Throw_UserNotFoundException()
        {
            var command = new VerifyUserEmailCommand("test@test.com", Guid.NewGuid());

            var handler = new VerifyUserEmailCommandHander(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task VerifyUserEmail_Should_Fail_When_Incorrect_Code()
        {
            var command = new VerifyUserEmailCommand("test@test.com", Guid.NewGuid());

            var handler = new VerifyUserEmailCommandHander(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(It.IsAny <Expression<Func<User, bool>>>(), null, true))
                .ReturnsAsync(new User());

            var result = await handler.Handle(command, default);


            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.IncorrectCode, result.Message);
        }


        [Fact]
        public async Task VerifyUserEmail_Should_Succeed()
        {
            var token = Guid.NewGuid();
            var command = new VerifyUserEmailCommand("test@test.com", token);

            var handler = new VerifyUserEmailCommandHander(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(
                x => x.GetByExpressionAsync(It.IsAny<Expression<Func<User, bool>>>(), null, true))
                .ReturnsAsync(new User() {  VerificationCode = token });

            var result = await handler.Handle(command, default);

            Assert.True(result.Success);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once());
        }

    }
}
