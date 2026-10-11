using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Api.DTOs.Category;

/*
    Se usa en Updates(PUT/PATCH)
    Permite enviar solo los campos que se quieren modificar
*/

// DTO para actualizar una categoria existente
public class UpdateCategoryDto
{
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    [RegularExpression(ValidationPatterns.CategoryName,
        ErrorMessage = "El nombre contiene caracteres no permitidos")]
    public string? Name { get; set; }
}