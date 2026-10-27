using Microsoft.EntityFrameworkCore;
using IncidentAPI.Api.Data;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;

namespace IncidentAPI.Api.Repositories.Implementations;

/*
    Este repositorio:
        - Usa entity framework para acceder a la BD
        - Maneja CRUD de comentarios
        - Permite obtener comentarios por incidente
*/

// Implementación del repositorio de comentarios usando Entity Framework
public class CommentRepository(AppDbContext context) : ICommentRepository
{

    public async Task<IEnumerable<Comment>> GetAllByIncidentIdAsync(int incidentId)
    {
        // Obtener un comentario de un incidente especifico
        return await context.Comments
        .Where(c => c.IncidentId == incidentId)
        .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        // Buscar comentario por id
        return await context.Comments.FindAsync(id);
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        // Agregar nuevo comentario
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> UpdateAsync(int id, Comment comment)
    {
        // Buscar comentario existente
        var existing = await context.Comments.FindAsync(id);
        if (existing == null) return null;

        // Actualizar datos
        existing.Content = comment.Content ?? existing.Content;
        existing.Author = comment.Author ?? existing.Author;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Buscar comentario
        var existing = await context.Comments.FindAsync(id);
        if (existing == null) return false;

        // Eliminar comentario
        context.Comments.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }


}