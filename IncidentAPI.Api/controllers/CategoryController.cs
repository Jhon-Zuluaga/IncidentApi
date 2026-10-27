using Microsoft.AspNetCore.Mvc;
using IncidentAPI.Api.DTOs.Category;
using IncidentAPI.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace IncidentAPI.Api.Controllers;

/// <summary>
/// Gestión de Categorias
/// </summary>
/// <param name="categoryService">Servicio de categorias</param>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{

    /// <summary>
    /// Obtiene todas las categorias
    /// </summary>
    /// <returns>Lista de categorias</returns>
    /// <response code="200">Retorna la lista</response>
    /// <response code="404">No hay categorias registradas</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAllAsync();
        if (!categories.Any()) return NotFound("No hay información suministrada");
        return Ok(categories);
    }

    /// <summary>
    /// Obtiene una categoria por su ID
    /// </summary>
    /// <param name="id">ID de la categoria</param>
    /// <returns>Categoria encontrada</returns>
    /// <response code="200">Retorna la categoria</response>
    /// <response code="404">Categoria no encontrada</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category == null) return NotFound($"Categoria con id {id} no encontrada");
        return Ok(category);
    }

    /// <summary>
    /// Crea una nueva categoria
    /// </summary>
    /// <param name="dto">Datos de la categoria a crear</param>
    /// <returns>Categoria creada</returns>
    /// <response code="201">Categoria creada correctamente</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza una categoria existente
    /// </summary>
    /// <param name="id">ID de la categoria a actualizar</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Categoria encontrada</returns>
    /// <response code="200">Datos a actualizar</response>
    /// <response code="400">Categoria no encontrada</response>
    /// <response code="404">Categoria no encontrada</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Debes enviar al menos un campo para actualizar");

        var updated = await categoryService.UpdateAsync(id, dto);
        if (updated == null) return NotFound($"Categoría con id {id} no encontrada");
        return Ok(updated);
    }

     /// <summary>
    /// Elimina una categoria por su ID
    /// </summary>
    /// <param name="id">ID de la categoria a eliminar</param>
    /// <returns>Mensaje de confirmación</returns>
    /// <response code="200">Categoria eliminada correctamente</response>
    /// <response code="404">Categoria no encontrada</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await categoryService.DeleteAsync(id);
        if (!deleted) return NotFound($"Categoria con id {id} no encontrada");
        return Ok($"Categoria con id {id} eliminado correctamente");
    }
}