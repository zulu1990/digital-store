using Application.Common.Persistance;
using Application.Common.Services;
using Domain;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;

namespace Application.Authorization.Commands.Passwords
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;


    internal class ForgotPasswordCommandHander : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;


        public ForgotPasswordCommandHander(IGenericRepository<User> userRepository, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x=> x.Email == request.Email);

            if (user == null)
                throw new UserNotFoundException($"{request.Email} user was not found");

            user.VerificationCode = Guid.NewGuid();

            await _emailSender.SendPasswordResetMail(user.Email, user.VerificationCode);

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }

}
