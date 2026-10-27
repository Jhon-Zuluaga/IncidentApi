using Microsoft.EntityFrameworkCore;
using IncidentAPI.Api.Data;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;

namespace IncidentAPI.Api.Repositories.Implementations;

/*
    Este repositorio:
        - Usa entity framework para acceder a la BD
        - Maneja CRUD de incidentes
        - Incluye relaciones con usuario, categoria y comentarios
*/

// Implementación del repositorio de incidentes usando Entity Framework
public class IncidentRepository(AppDbContext context) : IIncidentRepository
{

    public async Task<IEnumerable<Incident>> GetAllAsync()
    {
        // Obtener incidentes con sus relaciones
        return await context.Incidents
            .Include(i => i.User)
            .Include(i => i.Category)
            .Include(i => i.Comments)
            .ToListAsync();
    }

    public async Task<Incident?> GetByIdAsync(int id)
    {
        // Buscar incidente por id con relaciones
        return await context.Incidents
            .Include(i => i.User)
            .Include(i => i.Category)
            .Include(i => i.Comments)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Incident> CreateAsync(Incident incident)
    {
        // Agregar nuevo incidente
        context.Incidents.Add(incident);
        await context.SaveChangesAsync();
        return incident;
    }

    public async Task<Incident?> UpdateAsync(int id, Incident incident)
    {
        // Buscar incidente existente
        var existing = await context.Incidents.FindAsync(id);
        if (existing == null) return null;

        // Actualizar datos si se envian
        existing.Title = incident.Title ?? existing.Title;
        existing.Description = incident.Description != string.Empty ? incident.Description : existing.Description;
        existing.Status = incident.Status ?? existing.Status;
        existing.CategoryId = incident.CategoryId != 0 ? incident.CategoryId : existing.CategoryId;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Buscar incidente
        var existing = await context.Incidents.FindAsync(id);
        if (existing == null) return false;

        // Eliminar incidente
        context.Incidents.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }


}