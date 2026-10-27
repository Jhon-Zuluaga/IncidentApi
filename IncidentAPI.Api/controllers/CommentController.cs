using Microsoft.AspNetCore.Mvc;
using IncidentAPI.Api.DTOs.Comment;
using IncidentAPI.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace IncidentAPI.Api.Controllers;

/// <summary>
/// Gestión de Comentarios
/// </summary>
/// <param name="commentService">Servicio de comentarios</param>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentController(ICommentService commentService) : ControllerBase
{

    /// <summary>
    /// Obtiene todos los comentarios de un incidente
    /// </summary>
    /// <param name="incidentId">ID del incidente</param>
    /// <returns>Lista de comentarios del incidente</returns>
    /// <response code="200"> Retorna la lista</response>
    /// <response code="404"> No hay comentarios para ese incidente</response>
    [HttpGet("incident/{incidentId}")]
    public async Task<IActionResult> GetAllByIncidentId(int incidentId)
    {
        var comments = await commentService.GetAllByIncidentIdAsync(incidentId);
        if (!comments.Any()) return NotFound("No hay información suministrada");
        return Ok(comments);
    }

    /// <summary>
    /// Obtiene un comentario por su ID
    /// </summary>
    /// <param name="Id">ID del comentario</param>
    /// <returns>Comentario encontrado</returns>
    /// <response code="200"> Retorna el comentario</response>
    /// <response code="404"> Comentario no encontrado</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var comment = await commentService.GetByIdAsync(id);
        if (comment == null) return NotFound($"Comentario con id {id} no encontrado");
        return Ok(comment);
    }

    /// <summary>
    /// Crea un nuevo comentario
    /// </summary>
    /// <param name="dto">Datos del comentario a crear</param>
    /// <returns>Comentario creado</returns>
    /// <response code="201"> Comentario creado correctamente</response>
    /// <response code="400"> Datos inválidos</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await commentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza un comentario existente
    /// </summary>
    /// <param name="id">ID del comentario a actualizar</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Comentario actualizador</returns>
    /// <response code="200"> Comentario actualizado correctamente</response>
    /// <response code="400"> Datos inválidos o sin campos para actualizar</response>
    /// <response code="404"> Comentario no encontrado</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await commentService.UpdateAsync(id, dto);
        if (updated == null) return NotFound($"Comentario con id {id} no encontrado");
        return Ok(updated);
    }

    /// <summary>
    /// Elimina un comentario por su ID
    /// </summary>
    /// <param name="id">ID del comentario a eliminar</param>
    /// <returns>Mnesaje de confirmación</returns>
    /// <response code="200"> Comentario eliminado correctamente</response>
    /// <response code="400"> Comentario no encontrado</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await commentService.DeleteAsync(id);
        if (!deleted) return NotFound($"Comentario con id {id} no encontrado");
        return Ok($"Comentario con id {id} eliminado correctamente");
    }
}