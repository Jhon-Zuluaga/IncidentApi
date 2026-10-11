using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Api.DTOs.Category;

/*
    Se usa cuando el cliente crea una categoria
    Incluye validaciones para asaegurar datos correctos
*/

// DTO para crear una nueva categoria
public class CreateCategoryDto
{
    [Required]
    [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
    [MaxLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    [RegularExpression(ValidationPatterns.CategoryName,
        ErrorMessage = "El nombre contiene caracteres no permitidos")]
    public string Name { get; set; } = string.Empty;
}