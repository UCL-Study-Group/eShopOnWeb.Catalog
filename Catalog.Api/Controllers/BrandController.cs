using Catalog.Application.Services;
using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BrandController : ControllerBase
{
    private readonly BrandService _brandService;

    public BrandController(BrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogBrand>>> GetBrandsAsync()
    {
        var response = await _brandService.GetBrandsAsync();

        if (response is null)
            return NotFound();
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogBrand>> GetBrandAsync(string id)
    {
        var response = await _brandService.GetBrandAsync(id);
        
        if (response is null)
            return NotFound();
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogBrand>> CreateBrandAsync([FromQuery] CreateCatalogBrandDto brand)
    {
        var response = await _brandService.CreateBrandAsync(brand);

        return response is null ? Problem("Failed to create Brand, try again") : Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateBrandAsync([FromQuery] UpdateCatalogBrandDto brand)
    {
        var response = await _brandService.UpdateBrandAsync(brand);
        
        return response.IsFailed ? Problem("Failed to update brand, try again") : Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBrandAsync(string id)
    {
        var response = await _brandService.DeleteBrandAsync(id);
        
        return response.IsFailed ? Problem("Failed to delete brand, try again") : Ok();
    }
}