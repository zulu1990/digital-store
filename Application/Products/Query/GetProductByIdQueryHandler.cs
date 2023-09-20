using Domain;
using MediatR;

namespace Application.Products.Query
{
    internal class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result>
    {
        public Task<Result> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
