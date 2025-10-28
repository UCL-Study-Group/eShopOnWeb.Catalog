using FluentResults;

namespace Catalog.Application.Interfaces;

public interface ICatalogService<TCreateDto, TUpdateDto, TGetDto> 
{
    Task<Result<TGetDto>> CreateAsync(TCreateDto dto);
    Task<Result<TGetDto>> GetAsync(string id);
    Task<Result<IEnumerable<TGetDto>>> GetAllAsync(int? pageSize = null, int? pageIndex = null);
    Task<Result<TGetDto>> UpdateAsync(TUpdateDto dto);
    Task<Result> DeleteAsync(string id);
}