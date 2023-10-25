using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;

namespace Application.Authorization.Commands
{
    public record VerifyUserEmailCommand(string Email, Guid VerificationCode) : IRequest<Result>;

    internal class VerifyUserEmailCommandHander : IRequestHandler<VerifyUserEmailCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyUserEmailCommandHander(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(VerifyUserEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByExpressionAsync(x => x.Email == request.Email && x.EmailVerified == false);

            if (user == null)
            {
                throw new ArgumentException();
            }

            if(user.VerificationCode == request.VerificationCode)
            {
                user.EmailVerified = true;
                await _unitOfWork.CommitAsync();
                return Result.Succeed();
            }

            return Result.Fail("Code was incorrect");
        }
    }

}
