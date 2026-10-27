using IncidentAPI.Api.DTOs.User;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;
using IncidentAPI.Api.Services.Interfaces;

namespace IncidentAPI.Api.Services.Implementations;

/*
    Este service:
        - Maneja lógica de negocio
        - Convierte User -> DTO
        - Usa el repository para acceder a la BD
        - Encripta y valida contraseñas (JWT)
*/

// Implementación de la lógica de negocio para usuarios
public class UserService(IUserRepository userRepository) : IUserService
{

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        // Obtener usuarios desde el repositorio
        var users = await userRepository.GetAllAsync();

        // Convierte entidades (User) a DTOs
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        });
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        // Buscar un usuario por id
        var user = await userRepository.GetByIdAsync(id);
        if (user == null) return null;

        // Mapear a DTo 
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // Crear entidad User a partir del DTO
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            // Encriptar contraseña antes de guardar (JWT)
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        // Guardar en BD
        var created = await userRepository.CreateAsync(user);

        // Retornar DTO
        return new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email
        };
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
    {
        // Crear objeto con datos a actualizar
        var user = new User
        {
            Name = dto.Name ?? string.Empty,
            Email = dto.Email ?? string.Empty
        };

        // Actualizar en BD
        var updated = await userRepository.UpdateAsync(id, user);
        if (updated == null) return null;

        // Retornar DTO actualizado
        return new UserDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Email = updated.Email
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Eliminar usuario
        return await userRepository.DeleteAsync(id); 
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        // Buscar usuario por email
        var user = await userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        // Verificar contraseña encriptada
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }
}