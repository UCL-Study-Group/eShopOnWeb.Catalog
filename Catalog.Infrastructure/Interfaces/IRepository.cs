using FluentResults;

namespace Catalog.Infrastructure.Interfaces;

public interface IRepository<T>
{
    Task<Result<IEnumerable<T>>> GetAllAsync();
    Task<Result<T>> GetByIdAsync(int id);
    Task<Result> CreateAsync(T entity);
    Task<Result> UpdateAsync(T entity);
    Task<Result> DeleteAsync(int id);
}