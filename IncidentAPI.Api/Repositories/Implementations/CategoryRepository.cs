using Microsoft.EntityFrameworkCore;
using IncidentAPI.Api.Data;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;

namespace IncidentAPI.Api.Repositories.Implementations;

/*
    Este repositorio:
        - Usa entity framework para acceder a la BD
        - Maneja CRUD de categorias
*/

// Implementación del repositorio de categorias usando Entity Framework
public class CategoryRepository(AppDbContext context) : ICategoryRepository
{

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        // Obtener todas las categorias
        return await context.Categories.ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        // Buscar categorias por id
        return await context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        // Agregar nueva categoria
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(int id, Category category)
    {
        // Buscar categoria existente
        var existing = await context.Categories.FindAsync(id);
        if (existing == null) return null;

        // Actualizar nombre si se envia
        existing.Name = category.Name ?? existing.Name;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Buscar categoria
        var existing = await context.Categories.FindAsync(id);
        if (existing == null) return false;

        // Eliminar categoria
        context.Categories.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }


}