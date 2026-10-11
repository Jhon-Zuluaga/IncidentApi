using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Api.DTOs.Incident;

/*
    Se usa cuando el cliente crea un incidente
    Incluye validaciones para asegurar datos correctos
*/

// DTO para crear un nuevo incidente
public class CreateIncidentDto
{
    [Required]
    [MinLength(3, ErrorMessage = "El titulo debe tener al menos 3 caracteres")]
    [MaxLength(150, ErrorMessage = "El titulo no puede tener más de 150 caracteres")]
    [RegularExpression(ValidationPatterns.FreeText,
        ErrorMessage = "El título solo puede contener letras")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
    [MaxLength(2000, ErrorMessage = "La descripción no puede tener más de 2000 caracteres")]
    [RegularExpression(ValidationPatterns.FreeText,
        ErrorMessage = "La descripción contiene caracteres no permitidos")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [RegularExpression("abierto|en_progreso|cerrado",
        ErrorMessage = "El estado debe ser: abierto, en_progreso o cerrado")]
    public string Status { get; set; } = "abierto";

    [Required]
    public int UserId { get; set; }
    [Required]
    public int CategoryId { get; set; }

}