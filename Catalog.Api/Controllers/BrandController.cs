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
    public async Task<ActionResult<CatalogBrandListResponse>> GetBrandsAsync()
    {
        var response = await _brandService.GetAllAsync();

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);
        
        return Ok(new CatalogBrandListResponse
        {
            CatalogBrands = response.Value
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogBrandResponse>> GetBrandAsync(
        string id
        )
    {
        var response = await _brandService.GetAsync(id);
        
        if (response.IsFailed)
            return Problem(response.Errors[0].Message);
        
        if (response.Value == null)
            return NotFound();
        
        return Ok(new CatalogBrandResponse()
        {
            CatalogBrand = response.Value
        });
    }

    [HttpPost]
    public async Task<ActionResult<CatalogBrandResponse>> CreateBrandAsync(
        [FromBody] CreateCatalogBrandDto brand
        )
    {
        var response = await _brandService.CreateAsync(brand);

        if (response.IsFailed)
            return Problem(response.Errors[0].Message);
        
        return Ok(new CatalogBrandResponse()
        {
            CatalogBrand = response.Value
        });
    }

    [HttpPut]
    public async Task<ActionResult<CatalogBrandResponse>> UpdateBrandAsync(
        [FromBody] UpdateCatalogBrandDto brand
        )
    {
        var response = await _brandService.UpdateAsync(brand);
        
        if (response.IsFailed)
            return Problem(response.Errors[0].Message);

        return Ok(new CatalogBrandResponse()
        {
            CatalogBrand = response.Value
        });
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