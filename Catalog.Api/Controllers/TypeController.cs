using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Type;
using Catalog.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/catalog-types")]
public class TypeController : ControllerBase
{
    private readonly ITypeService _typeService;

    public TypeController(ITypeService typeService)
    {
        _typeService = typeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCatalogTypeDto>>> GetTypesAsync()
    {
        var response = await _typeService.GetAllAsync();

        if (response is null)
            return NotFound("Collection does not exists");
        
        return Ok(response.Select(GetCatalogTypeDto.FromModel));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCatalogTypeDto?>> GetTypeAsync(
        string id
        )
    {
        var response = await _typeService.GetAsync(id);

        if (response is null)
            return NotFound();
        
        return Ok(GetCatalogTypeDto.FromModel(response));
    }

    [HttpPost]
    public async Task<ActionResult<CatalogType>> PostTypeAsync(
        [FromQuery] CreateCatalogTypeDto model
        )
    {
        var response = await _typeService.CreateAsync(model);

        return response is null ? Problem("Failed to create CatalogType") : Ok(response);
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateTypeAsync(
        [FromQuery] UpdateCatalogTypeDto type
        )
    {
        var response = await _typeService.UpdateAsync(type);
        
        return response.IsFailed ? Problem("Failed to update brand, try again") : Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTypeAsync(
        string id
        )
    {
        var response = await _typeService.DeleteAsync(id);
        
        return response.IsFailed ? Problem("Failed to delete brand, try again") : Ok();
    }
}