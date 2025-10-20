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
    public async Task<ActionResult<IEnumerable<GetCatalogItemDto>>> GetCatalogItemsAsync()
    {
        var response = await _catalogService.GetAllAsync();

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCatalogItemDto>> GetCatalogItemAsync(string id)
    {
        var response = await _catalogService.GetByIdAsync(id);

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogItem>> CreateCatalogItemAsync([FromQuery] CreateCatalogItemDto item)
    {
        var response = await _catalogService.CreateAsync(item);

        return response is null ? Problem("Failed to create catalog item. Try again") : Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCatalogItemAsync([FromQuery] UpdateCatalogItemDto item)
    {
        var response = await _catalogService.UpdateAsync(item);
        
        return response.IsFailed ? Problem("Failed to update catalog item. Try again") : Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCatalogItemAsync(string id)
    {
        var response = await _catalogService.DeleteAsync(id);
        
        return response.IsFailed ? Problem("Failed to delete item. Try again") : Ok();
    }
}