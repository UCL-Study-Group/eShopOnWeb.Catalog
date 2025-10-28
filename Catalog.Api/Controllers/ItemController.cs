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
    public async Task<ActionResult<CatalogItemListResponse>> GetCatalogItemsAsync(
        int? pageSize, 
        int? pageIndex,
        string? brandId,
        string? typeId
        )
    {
        var response = await _itemService.GetAllAsync(pageSize, pageIndex, brandId, typeId);

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);
        
        return Ok(new CatalogItemListResponse { CatalogItems = response.Value });
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogItemListResponse>> GetCatalogItemAsync(
        string id
        )
    {
        var response = await _itemService.GetAsync(id);
        
        if (response.IsFailed)
            return Problem(response.Errors[0].Message);
        
        return Ok(new GetCatalogItemResponse()
        {
            CatalogItem = response.Value
        });
    }

    [HttpPost]
    public async Task<ActionResult<GetCatalogItemResponse>> CreateCatalogItemAsync(
        [FromBody] CreateCatalogItemDto item
        )
    {
        var response = await _itemService.CreateAsync(item);
        
        if (response.IsFailed)
            return Problem(response.Errors[0].Message);

        return Ok(new GetCatalogItemResponse()
        {
            CatalogItem = response.Value
        });
    }

    [HttpPut]
    public async Task<ActionResult<CatalogItemListResponse>> UpdateCatalogItemAsync(
        [FromBody] UpdateCatalogItemDto item
        )
    {
        var response = await _itemService.UpdateAsync(item);
        
        if (response.IsFailed)
            return Problem(response.Errors[0].Message);
        
        return Ok(new GetCatalogItemResponse()
        {
            CatalogItem = response.Value
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<StatusDto>> DeleteCatalogItemAsync(string id)
    {
        var response = await _itemService.DeleteAsync(id);
        
        return response.IsFailed ? Problem("Failed to delete item. Try again") : Ok(new StatusDto()
        {
            Status = "Deleted"
        });
    }
}