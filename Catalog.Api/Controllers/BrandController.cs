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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogBrand>> GetBrandAsync(int id)
    {
        var response = await _brandService.GetBrandAsync(id);
        
        if (response is null)
            return NotFound();
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBrandAsync(CreateCatalogBrandDto brand)
    {
        var response = await _brandService.CreateBrandAsync(brand);

        return response is null ? Problem("Failed to create Brand, try again") : Ok(response);
    }
}