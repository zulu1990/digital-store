using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;

namespace Application.AdminPanel.Commands
{
    public record BanUserCommand(Guid UserId, string Reason) : IRequest<Result>;


    internal class BanUserCommandHandler : IRequestHandler<BanUserCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BanUserCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
                throw new UserNotFoundException(ErrorMessages.UserNotFound);

            if (user.Ban)
                throw new UserIsAlreadyBannedException();

            user.Ban = true;
            user.BanReason = request.Reason;

            await _unitOfWork.CommitAsync();
            return Result.Succeed();
        }
    }

}
