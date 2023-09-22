using Application.Common.Handler;
using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Authorization.Commands
{
    public record class RegisterUserCommand(string Email, string Password) : IRequest<Result<string>>;


    internal class RegisterUserCommandHander : IRequestHandler<RegisterUserCommand, Result<string>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHander _passwordHander;
        private readonly IJwtTokenHander _jwtTokenHander;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHander(IGenericRepository<User> userRepository, IPasswordHander passwordHander,
            IJwtTokenHander jwtTokenHander, IUnitOfWork unitOfWork)
        {
            _jwtTokenHander = jwtTokenHander;
            _userRepository = userRepository;
            _passwordHander = passwordHander;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByExpressionAsync(x=> x.Email == request.Email);

            if (existingUser is not null)
                return Result<string>.Fail("User Already Exists", StatusCodes.Status400BadRequest);

            _passwordHander.CreateSaltAndHash(request.Password, out var passwordHash, out var passwordSalt);

            var newUser = new User()
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = request.Email,
                Orders = new List<Order>()
            };

            var user = await _userRepository.AddAsync(newUser);
            await _unitOfWork.CommitAsync();


            var token = _jwtTokenHander.CreateToken(user);

            return Result<string>.Succeed(token);
        }
    }

}
