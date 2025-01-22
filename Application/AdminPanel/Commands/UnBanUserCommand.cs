using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Commands
{
    public record UnBanUserCommand(Guid UserId) : IRequest<Result>;

    internal class UnBanUserCommandHandler : IRequestHandler<UnBanUserCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnBanUserCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(UnBanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
                throw new UserNotFoundException(ErrorMessages.UserNotFound);

            if (!user.Ban)
                return Result.Fail(ErrorMessages.UserWasNotBanned);

            user.Ban = false;
            user.BanReason = string.Empty;

            await _unitOfWork.CommitAsync();
            return Result.Succeed();
        }
    }
}
