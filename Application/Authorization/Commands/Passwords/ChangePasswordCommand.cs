using Application.Common.Handler;
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

namespace Application.Authorization.Commands.Passwords
{
    public record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword) : IRequest<Result>;


    internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHander _passwordHander;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IGenericRepository<User> userRepository, IPasswordHander passwordHander, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHander = passwordHander;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
                throw new UserNotFoundException(ErrorMessages.UserNotFound);

            if (_passwordHander.ValidatePassword(request.OldPassword, user.PasswordHash, user.PasswordSalt) == false)
                throw new IncorrectPasswordException();

            _passwordHander.CreateSaltAndHash(request.NewPassword, out var newHash, out var newSalt);

            user.PasswordSalt = newSalt;
            user.PasswordHash = newHash;

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
