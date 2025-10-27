using FluentResults;

namespace Catalog.Infrastructure.Repositories.Interfaces;

public interface IDbRepository<TEntity, TGet>
{
    /// <summary>
    /// Retrieves all entities of the provided type from the database
    /// </summary>
    /// <returns>A collection of entities</returns>
    Task<Result<IEnumerable<TGet>>> GetAllAsync(int? pageSize = null, int? pageIndex = null, string? brandId = null, string? typeId = null);
    
    /// <summary>
    /// Retrieves the first entity with the provided id.
    /// </summary>
    /// <param name="id">The id of the type</param>
    /// <returns>The entity if found</returns>
    Task<Result<TGet>> GetByIdAsync(string id);
    
    /// <summary>
    /// Retrieves the first entity with the provided id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Obsolete("Get by legacy will be replaced in the future")]
    Task<Result<TGet>> GetByLegacyIdAsync(int id);
    
    
    /// <summary>
    /// Creates a new entity of the provided type and saves it
    /// </summary>
    /// <param name="entity">The entity that is to be created</param>
    /// <returns>A result indicating success or failure</returns>
    Task<Result<TGet>> CreateAsync(TEntity entity);
    
    
    /// <summary>
    /// Updates an already existing entity with provided one
    /// </summary>
    /// <param name="entity">The entity which should be updated</param>
    /// <returns>A result indicating success or failure</returns>
    Task<Result<TGet>> UpdateAsync(TEntity entity);
    
    
    /// <summary>
    /// Removes the entity matching the id provided 
    /// </summary>
    /// <param name="id">The id of the entity to be deleted</param>
    /// <returns>A result indicating success or failure</returns>
    Task<Result> DeleteAsync(string id);
}