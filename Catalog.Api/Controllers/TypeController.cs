using Catalog.Application.Services;
using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TypeController : ControllerBase
{
    private readonly TypeService _typeService;

    public TypeController(TypeService typeService)
    {
        _typeService = typeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogType>>> GetTypesAsync()
    {
        var response = await _typeService.GetTypesAsync();

        if (response is null)
            return NotFound("Collection does not exists");
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogType?>> GetTypeAsync(string id)
    {
        var response = await _typeService.GetTypeAsync(id);

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogType>> PostTypeAsync([FromBody] CreateCatalogTypeDto model)
    {
        var response = await _typeService.CreateTypeAsync(model);

        return response is null ? Problem("Failed to create CatalogType") : Ok(response);
    }
}