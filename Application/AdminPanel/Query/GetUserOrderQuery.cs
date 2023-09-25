using Application.Common.Persistance;
using Domain;
using Domain.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminPanel.Query
{
    public record GetUserOrderQuery(Guid UserId): IRequest<Result<IEnumerable<Order>>>;



    internal class GetUserOrderQueryHandler : IRequestHandler<GetUserOrderQuery, Result<IEnumerable<Order>>>
    {
        private readonly IGenericRepository<User> _repository;

        public GetUserOrderQueryHandler(IGenericRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<Order>>> Handle(GetUserOrderQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByExpressionAsync(x => x.Id == request.UserId, includes: "Orders");

            return Result<IEnumerable<Order>>.Succeed(user.Orders);
        }
    }
}
