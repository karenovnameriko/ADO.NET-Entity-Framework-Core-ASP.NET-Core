using Microsoft.AspNetCore.Mvc;

namespace KT5_WebAPITemp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _unitOfWork.Categories.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id) return BadRequest();

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}