using IncidentAPI.Api.DTOs.Incident;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;
using IncidentAPI.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncidentAPI.Api.Services.Implementations;

/*
    Este service:
        - Maneja logica de negocio
        - Convierte Incident -> DTO
        - Usa el repository para acceder a la BD
        - Valida existencia de usuario y categoria
        - Envia notificación por email
*/

// Implementación de la logica de negocio para incidentes
public class IncidentService(
    IIncidentRepository incidentRepository,
    IUserRepository userRepository,
    ICategoryRepository categoryRepository,
    IEmailService emailService,
    ILogger<IncidentService> logger) : IIncidentService
{

    public async Task<IEnumerable<IncidentDto>> GetAllAsync()
    {
        // Obtener incidentes
        var incidents = await incidentRepository.GetAllAsync();

        // Convertir a DTO
        return incidents.Select(i => new IncidentDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            Status = i.Status,
            CreatedAt = i.CreatedAt,
            UserId = i.UserId,
            UserName = i.User?.Name ?? string.Empty,
            CategoryId = i.CategoryId,
            CategoryName = i.Category?.Name ?? string.Empty
        });
    }

    public async Task<IncidentDto?> GetByIdAsync(int id)
    {
        // Buscar incidente por id
        var Incident = await incidentRepository.GetByIdAsync(id);
        if (Incident == null) return null;

        // Mapear a DTO
        return new IncidentDto
        {
            Id = Incident.Id,
            Title = Incident.Title,
            Description = Incident.Description,
            Status = Incident.Status,
            CreatedAt = Incident.CreatedAt,
            UserId = Incident.UserId,
            UserName = Incident.User?.Name ?? string.Empty,
            CategoryId = Incident.CategoryId,
            CategoryName = Incident.Category?.Name ?? string.Empty
        };
    }

    public async Task<IncidentDto> CreateAsync(CreateIncidentDto dto)
    {
        // Verificar que el usuario existe
        var user = await userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new BadRequestException($"No existe un usuario con id {dto.UserId}");

        // Verificar que la categoría existe
        var category = await categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new BadRequestException($"No existe una categoría con id {dto.CategoryId}");

        // Crear entidad incident
        var incident = new Incident
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            UserId = dto.UserId,
            CategoryId = dto.CategoryId
        };

        // Guardar en BD
        var created = await incidentRepository.CreateAsync(incident);

        // Obtener con relaciones completas
        var full = await incidentRepository.GetByIdAsync(created.Id);

        try
        {
            // Enviar correo de notificacion
            await emailService.SendIncidentCreatedAsync(user.Email, user.Name, dto.Title);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enviando notificación de incidente creado a  {Email} para el incidente {IncidentId}", 
            user.Email, created.Id);
        }

        // Retornar DTO
        return new IncidentDto
        {
            Id = full!.Id,
            Title = full.Title,
            Description = full.Description,
            Status = full.Status,
            CreatedAt = full.CreatedAt,
            UserId = full.UserId,
            UserName = full.User?.Name ?? string.Empty,
            CategoryId = full.CategoryId,
            CategoryName = full.Category?.Name ?? string.Empty
        };
    }

    public async Task<IncidentDto?> UpdateAsync(int id, UpdateIncidentDto dto)
    {
        // Crear entidad con datos a actualizar
        var incident = new Incident
        {
            Title = dto.Title ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Status = dto.Status ?? string.Empty,
            CategoryId = dto.CategoryId.GetValueOrDefault()
        };

        // Actualizar en BD
        var updated = await incidentRepository.UpdateAsync(id, incident);
        if (updated == null) return null;

        // Obtener completo
        var full = await incidentRepository.GetByIdAsync(updated.Id);

        // Retornar DTO
        return new IncidentDto
        {
            Id = full!.Id,
            Title = full.Title,
            Description = full.Description,
            Status = full.Status,
            CreatedAt = full.CreatedAt,
            UserId = full.UserId,
            UserName = full.User?.Name ?? string.Empty,
            CategoryId = full.CategoryId,
            CategoryName = full.Category?.Name ?? string.Empty
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Eliminar incidente
        return await incidentRepository.DeleteAsync(id);
    }
}