using Domain;
using MediatR;

namespace Application.Products.Query;

public record GetProductByIdQuery(Guid ProductId) : IRequest<Result>;


