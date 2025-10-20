using Catalog.Application.Services;
using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly CatalogService _catalogService;

    public ItemController(CatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItemCreateDto>>> GetCatalogItemsAsync()
    {
        var response = await _catalogService.GetAllAsync();

        if (!response.Any())
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogItemCreateDto>> GetCatalogItemAsync(int id)
    {
        var response = await _catalogService.GetByIdAsync(id);

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogItemCreateDto>> CreateCatalogItemAsync([FromBody] CatalogItem item)
    {
        var response = await _catalogService.CreateAsync(item);

        return response is null ? Problem("Failed to create catalog item. Try again") : Ok(response);
    }
}