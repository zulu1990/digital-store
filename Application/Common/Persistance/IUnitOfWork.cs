namespace Application.Common.Persistance;

public interface IUnitOfWork
{
    Task<bool> CommitAsync();
}
