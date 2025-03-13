using FluentResults;

namespace HomeIMS.SharedContracts.EventSourcing;

public interface IReadModelAccess<TEntity>
    where TEntity : class
{
    Task<Result<TEntity?>> GetById(Guid id);
    Task<Result<IEnumerable<TEntity>>> GetAll();
}