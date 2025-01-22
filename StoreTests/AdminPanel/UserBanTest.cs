using Application.AdminPanel.Commands;
using Application.Common.Persistance;
using Domain.Entity;
using Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoreTests.AdminPanel
{
    public class UserBanTest
    {
        private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public UserBanTest()
        {
            _userRepositoryMock = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task UserBan_Should_Throw_UserIsAlreadyBannedException()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Ban = true
                });

            var command = new BanUserCommand(userId, "test");

            var handler = new BanUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            await Assert.ThrowsAsync<UserIsAlreadyBannedException>(() => handler.Handle(command, default));
        }

        [Fact]
        public async Task UserBan_Should_Ban_User()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Ban = false
                });

            var command = new BanUserCommand(userId, "test");

            var handler = new BanUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            var result = await handler.Handle(command, default);

            Assert.True(result.Success);
            _unitOfWorkMock.Verify(x=> x.CommitAsync(), Times.Once());
        }

    }
}
