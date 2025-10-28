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
    public async Task<ActionResult<CatalogTypeListResponse>> GetTypesAsync()
    {
        var response = await _typeService.GetAllAsync();

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);

        return Ok(new CatalogTypeListResponse
        {
            CatalogTypes = response.Value
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCatalogTypeDto?>> GetTypeAsync(
        string id
    )
    {
        var response = await _typeService.GetAsync(id);

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);

        return Ok(response.Value);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogTypeResponse>> PostTypeAsync(
        [FromBody] CreateCatalogTypeDto model
    )
    {
        var response = await _typeService.CreateAsync(model);

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);

        return Ok(new CatalogTypeResponse()
        {
            CatalogTypes = response.Value
        });
    }

    [HttpPut]
    public async Task<ActionResult<CatalogTypeResponse>> UpdateTypeAsync(
        [FromBody] UpdateCatalogTypeDto type
    )
    {
        var response = await _typeService.UpdateAsync(type);

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);

        return Ok(new CatalogTypeResponse()
        {
            CatalogTypes = response.Value
        });
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