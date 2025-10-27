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
    public async Task<ActionResult<GetCatalogBrandObjectDto>> GetBrandsAsync()
    {
        var response = await _brandService.GetAllAsync();

        if (response is null)
            return NotFound();
        
        return Ok(new GetCatalogBrandObjectDto { CatalogBrands = response.Select(GetCatalogBrandDto.FromModel)});
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
    public async Task<ActionResult<GetCatalogBrandObjectDto>> CreateBrandAsync(
        [FromBody] CreateCatalogBrandDto brand
        )
    {
        var response = await _brandService.CreateAsync(brand);

        return response is null ? Problem("Failed to create Brand, try again") : Ok(new
        {
            CatalogBrands = new[] { response }
        });
    }

    [HttpPut]
    public async Task<ActionResult<GetCatalogBrandObjectDto>> UpdateBrandAsync(
        [FromBody] UpdateCatalogBrandDto brand
        )
    {
        var response = await _brandService.UpdateAsync(brand);
        
        return response.IsFailed ? Problem("Failed to update brand, try again") : Ok(response.Value);
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