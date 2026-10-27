using IncidentAPI.Api.DTOs.Comment;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;
using IncidentAPI.Api.Services.Interfaces;

namespace IncidentAPI.Api.Services.Implementations;


/*
    Este service:
        - Maneja logica de negocio
        - Convierte comment -> DTO
        - Usa el repository para acceder a la BD
*/

// Implementacion de la logica de negocio para comentarios
public class CommentService(ICommentRepository commentRepository) : ICommentService
{

    public async Task<IEnumerable<CommentDto>> GetAllByIncidentIdAsync(int incidentId)
    {
        // Obtener comentarios por incidente
        var comments = await commentRepository.GetAllByIncidentIdAsync(incidentId);

        // Convertir a DTO
        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            Author = c.Author,
            CreatedAt = c.CreatedAt,
            IncidentId = c.IncidentId
        });
    }

    public async Task<CommentDto?> GetByIdAsync(int id)
    {
        // Buscar comentario por ID
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment == null) return null;

        // Mapear a DTO
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            Author = comment.Author,
            CreatedAt = comment.CreatedAt,
            IncidentId = comment.IncidentId
        };
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
    {
        // Crear entidad Comment
        var comment = new Comment
        {
            Content = dto.Content,
            Author = dto.Author,
            IncidentId = dto.IncidentId
        };

        // Guardar en la BD
        var created = await commentRepository.CreateAsync(comment);

        // Retornar el DTO
        return new CommentDto
        {
            Id = created.Id,
            Content = created.Content,
            Author = created.Author,
            CreatedAt = created.CreatedAt,
            IncidentId = created.IncidentId
        };
    }

    public async Task<CommentDto?> UpdateAsync(int id, UpdateCommentDto dto)
    {
        // Crear entidad con datos a actualizar
        var comment = new Comment
        {
            Content = dto.Content ?? string.Empty,
            Author = dto.Author ?? string.Empty
        };

        // Actualizar en BD
        var updated = await commentRepository.UpdateAsync(id, comment);
        if (updated == null) return null;

        // Retornar DTO
        return new CommentDto
        {
            Id = updated.Id,
            Content = updated.Content,
            Author = updated.Author,
            CreatedAt = updated.CreatedAt,
            IncidentId = updated.IncidentId
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Eliminar comentario
        return await commentRepository.DeleteAsync(id);
    }
}