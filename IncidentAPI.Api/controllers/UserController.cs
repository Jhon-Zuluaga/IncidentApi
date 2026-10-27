using Microsoft.AspNetCore.Mvc;
using IncidentAPI.Api.DTOs.User;
using IncidentAPI.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace IncidentAPI.Api.Controllers;

/// <summary>
/// Gestión de Usuarios
/// </summary>
/// <param name="userService">Servicio de usuarios</param>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{

    /// <summary>
    /// Obtiene todos los usuarios
    /// </summary>
    /// <returns>Lista de usuarios</returns>
    /// <response code="200">Retorna la lista</response>
    /// <response code="404">No hay usuarios registrados</response>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await userService.GetAllAsync();
        if (!users.Any()) return NotFound("No hay información suministrada");
        return Ok(users);
    }

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Usuario encontrado</returns>
    /// <response code="200">Retorna el usuario</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user == null) return NotFound($"Usuario con id {id} no encontrado");
        return Ok(user);
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    /// <param name="dto">Datos del usuario a crear</param>
    /// <returns>Usuario creado</returns>
    /// <response code="201">Usuario creado correctamente</response>
    /// <response code="404">Datos inválidos</response>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Usuario actualizado</returns>
    /// <response code="200">Usuario actualizado correctamente</response>
    /// <response code="400">Datos inválidos o sin campos para actualizar</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await userService.UpdateAsync(id, dto);
        if (updated == null) return NotFound($"Usuario con id {id} no encontrado");
        return Ok(updated);
    }

    /// <summary>
    /// Elimina un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario a eliminar</param>
    /// <returns>Mensaje de confirmación</returns>
    /// <response code="200">Usuario eliminado correctamente</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await userService.DeleteAsync(id);
        if (!deleted) return NotFound($"Usuario con id {id} no encontrado");
        return Ok($"Usuario con id {id} eliminado correctamente");
    }
}