using Microsoft.EntityFrameworkCore;
using IncidentAPI.Api.Data;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;

namespace IncidentAPI.Api.Repositories.Implementations;

/*
    Este repositorio: 
        - Usa entity framework para acceder a la BD
        - Maneja CRUD de usuarios
        - Gestiona tokens de recuperación (reset password)
*/

// Implementación del repositorio de usuarios usando Entity Framework
public class UserRepository(AppDbContext context) : IUserRepository
{

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        // Obtener todos los usuarios de la BD
        return await context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        // Buscar por id
        return await context.Users.FindAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        // Agregar nuevo usuario
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(int id, User user)
    {
        // Buscar usuario existente
        var existing = await context.Users.FindAsync(id);
        if (existing == null) return null;

        // Actualizar datos básicos si vienen en el request
        existing.Name = user.Name ?? existing.Name;
        existing.Email = user.Email ?? existing.Email;

        // Actualizar contraseña solo si se envia
        if(!string.IsNullOrEmpty(user.PasswordHash))
            existing.PasswordHash = user.PasswordHash;
        
        existing.ResetToken = user.ResetToken;
        existing.ResetTokenExpiry = user.ResetTokenExpiry;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {   
        // Buscar usuario
        var existing = await context.Users.FindAsync(id);
        if (existing == null) return false;

        // Eliminar usuario
        context.Users.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // Buscar usuario por email(login/recuperación)
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}