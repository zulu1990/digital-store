using Application.Common.Handler;
using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;

namespace Application.Authorization.Commands.Passwords
{
    public record ResetPasswordCommand(string Email, string Password, Guid Token) : IRequest<Result>;


    internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHander _passwordHander;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(IGenericRepository<User> userRepository, IPasswordHander passwordHander, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHander = passwordHander;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x=> x.Email == request.Email);

            if (user == null)
                throw new UserNotFoundException(ErrorMessages.UserNotFound);
            
            if(user.VerificationCode == request.Token)
            {
                _passwordHander.CreateSaltAndHash(request.Password, out var newHash, out var newSalt);
                user.PasswordSalt = newSalt;
                user.PasswordHash = newHash;

                await _unitOfWork.CommitAsync();

                return Result.Succeed();
            }

            throw new InvalidTokenException();

        }
    }


}
