using Application.Common.Handler;
using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Authorization.Queries
{
    public record LoginQuery (string Email, string Password) : IRequest<Result<string>>;



    internal class LoginQueryHandler : IRequestHandler<LoginQuery, Result<string>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHander _passwordHander;
        private readonly IJwtTokenHander _jwtTokenHander;

        public LoginQueryHandler(IGenericRepository<User> userRepository, IPasswordHander passwordHander,
            IJwtTokenHander jwtTokenHander)
        {
            _jwtTokenHander = jwtTokenHander;
            _userRepository = userRepository;
            _passwordHander = passwordHander;
        }

        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByExpressionAsync(x=> x.Email == request.Email, trackChanges: false);

            if (existingUser is null)
                return Result<string>.Fail(ErrorMessages.IncorrectCredentials, StatusCodes.Status404NotFound);

            if (!_passwordHander.ValidatePassword(request.Password, existingUser.PasswordHash, existingUser.PasswordSalt))
                return Result<string>.Fail(ErrorMessages.IncorrectCredentials);

            var token = _jwtTokenHander.CreateToken(existingUser);

            return Result<string>.Succeed(token);
        }
    }
}
