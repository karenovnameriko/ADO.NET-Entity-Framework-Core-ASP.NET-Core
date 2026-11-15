using Microsoft.AspNetCore.Mvc;

namespace KT5_WebAPITemp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _unitOfWork.Products.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id) return BadRequest();

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        _unitOfWork.Products.Delete(product);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}