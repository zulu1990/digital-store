using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using Domain.Enum;
using Domain.Exceptions;
using MediatR;

namespace Application.Customer.Commands
{
    public record EditUserCommand (Guid UserId, Role? Role, string? Currency, string? Address) : IRequest<Result>;


    internal class EditUserCommandHandler : IRequestHandler<EditUserCommand, Result>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditUserCommandHandler(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (!string.IsNullOrWhiteSpace(request.Currency))
                user.Currency = request.Currency;

            if (request.Role.HasValue)
                user.Role = request.Role.Value;

            if(!string.IsNullOrWhiteSpace(request.Address))
                user.Address = request.Address;

            await _unitOfWork.CommitAsync();

            return Result.Succeed();
        }
    }
}
