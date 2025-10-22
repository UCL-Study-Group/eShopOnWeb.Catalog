using FluentResults;

namespace Catalog.Application.Interfaces;

public interface ICatalogService<TEntity, TCreateDto, TUpdateDto, TGetDto> 
    where TEntity : class
{
    Task<TEntity?> CreateAsync(TCreateDto dto);
    Task<TGetDto?> GetAsync(string id);
    Task<IEnumerable<TGetDto>?> GetAllAsync(int? pageSize = null, int? pageIndex = null);
    Task<Result> UpdateAsync(TUpdateDto dto);
    Task<Result> DeleteAsync(string id);
}