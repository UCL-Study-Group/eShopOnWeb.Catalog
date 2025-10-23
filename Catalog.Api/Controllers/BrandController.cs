using Catalog.Application.Interfaces;
using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Brand;
using Catalog.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/catalog-brands")]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCatalogBrandDto>>> GetBrandsAsync()
    {
        var response = await _brandService.GetAllAsync();

        if (response is null)
            return NotFound();
        
        return Ok(response.Select(GetCatalogBrandDto.FromModel));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogBrand>> GetBrandAsync(
        string id
        )
    {
        var response = await _brandService.GetAsync(id);
        
        if (response is null)
            return NotFound();
        
        return Ok(GetCatalogBrandDto.FromModel(response));
    }

    [HttpPost]
    public async Task<ActionResult<CatalogBrand>> CreateBrandAsync(
        [FromQuery] CreateCatalogBrandDto brand
        )
    {
        var response = await _brandService.CreateAsync(brand);

        return response is null ? Problem("Failed to create Brand, try again") : Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateBrandAsync(
        [FromQuery] UpdateCatalogBrandDto brand
        )
    {
        var response = await _brandService.UpdateAsync(brand);
        
        return response.IsFailed ? Problem("Failed to update brand, try again") : Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBrandAsync(
        string id
        )
    {
        var response = await _brandService.DeleteAsync(id);
        
        return response.IsFailed ? Problem("Failed to delete brand, try again") : Ok();
    }
}