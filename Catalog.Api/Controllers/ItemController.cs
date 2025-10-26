using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Item;
using Catalog.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/catalog-items")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult<GetCatalogItemsListDto>> GetCatalogItemsAsync(
        int? pageSize, 
        int? pageIndex,
        string? brandId,
        string? typeId
        )
    {
        var response = await _itemService.GetAllAsync(pageSize, pageIndex, brandId, typeId);

        if (response is null)
            return NotFound();
        
        return Ok(new GetCatalogItemsListDto { CatalogItems = response });
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCatalogItemAsync(
        string id
        )
    {
        var response = await _itemService.GetAsync(id);

        if (response is null)
            return NotFound();
        
        return Ok(new
        {
            CatalogItems = new[] { response }
        });
    }

    [HttpPost]
    public async Task<ActionResult<GetCatalogItemsListDto>> CreateCatalogItemAsync(
        [FromBody] CreateCatalogItemDto item
        )
    {
        var response = await _itemService.CreateAsync(item);

        return response is null ? Problem("Failed to create catalog item. Try again") : Ok(new
        {
            CatalogItems = new[] { response }
        });
    }

    [HttpPut]
    public async Task<ActionResult<GetCatalogItemsListDto>> UpdateCatalogItemAsync(
        [FromBody] UpdateCatalogItemDto item
        )
    {
        var response = await _itemService.UpdateAsync(item);
        
        return response.IsFailed ? Problem("Failed to update catalog item. Try again") : Ok(response.Value);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCatalogItemAsync(string id)
    {
        var response = await _itemService.DeleteAsync(id);
        
        return response.IsFailed ? Problem("Failed to delete item. Try again") : Ok();
    }
}