using Microsoft.AspNetCore.Mvc;
using IncidentAPI.Api.DTOs.Incident;
using IncidentAPI.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace IncidentAPI.Api.Controllers;

/// <summary>
/// Gestión de Incidentes
/// </summary>
/// <param name="incidentService">Servicio de incidentes</param>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IncidentController(IIncidentService incidentService) : ControllerBase
{

    /// <summary>
    /// Obtiene todos los incidentes
    /// </summary>
    /// <returns>Lista de incidentes</returns>
    /// <response code="200">Retorna la lista</response>
    /// <response code="404">No hay incidentes registrados</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var incidents = await incidentService.GetAllAsync();
        if (!incidents.Any()) return NotFound("No hay información suministrada");
        return Ok(incidents);
    }

    /// <summary>
    /// Obtiene un incidente por su ID
    /// </summary>
    /// <param name="id">ID del incidente</param>
    /// <returns>Incidente encontrado</returns>
    /// <response code="200">Retorna el incidente</response>
    /// <response code="404">Incidente no encontrado</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var incident = await incidentService.GetByIdAsync(id);
        if (incident == null) return NotFound($"Incidente con id {id} no encontrado");
        return Ok(incident);
    }

    /// <summary>
    /// Crea un nuevo incidente
    /// </summary>
    /// <param name="id">Datos del incidente a crear</param>
    /// <returns>Incidente creado</returns>
    /// <response code="201">Incidente creado correctamente</response>
    /// <response code="404">Datos inválidos</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIncidentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await incidentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza un incidente existente
    /// </summary>
    /// <param name="id">ID del incidente a actualizar</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Incidente actualizado</returns>
    /// <response code="200">Incidente actualizado correctamente</response>
    /// <response code="400">Datos inválidos o sin campos para actualizar</response>
    /// <response code="404">Incidente no encontrado</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIncidentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await incidentService.UpdateAsync(id, dto);
        if (updated == null) return NotFound($"Incidente con id {id} no encontrado");
        return Ok(updated);
    }

    /// <summary>
    /// Elimina un incidente por su ID
    /// </summary>
    /// <param name="id">ID del incidente a eliminar</param>
    /// <returns>Mensaje de confirmación</returns>
    /// <response code="200">Incidente eliminado correctamente</response>
    /// <response code="404">Incidente no encontrado</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await incidentService.DeleteAsync(id);
        if (!deleted) return NotFound($"Incidente con id {id} no encontrado");
        return Ok($"Incidente con id {id} eliminado correctamente");
    }
}