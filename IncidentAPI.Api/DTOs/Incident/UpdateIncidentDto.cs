using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Api.DTOs.Incident;


/*
    Se usa en updates (PUT/PATCH)
    Permite modificar solo los campos necesarios con validaciones
*/

// DTO para actualizar un incidente existente
public class UpdateIncidentDto
{
    [MinLength(3, ErrorMessage = "El titulo debe tener al menos 3 caracteres")]
    [MaxLength(150, ErrorMessage = "El titulo no puede tener más de 150 caracteres")]
    [RegularExpression(ValidationPatterns.FreeText,
        ErrorMessage = "El título contiene caracteres no permitidos")]
    public string? Title { get; set; }

    [MinLength(3, ErrorMessage = "La descripción debe tener al menos 3 caracteres")]
    [MaxLength(2000, ErrorMessage = "La descripción no puede tener más de 2000 caracteres")]
    [RegularExpression(ValidationPatterns.FreeText,
        ErrorMessage = "La descripción contiene caracteres no permitidos")]
    public string? Description { get; set; }
    
   
    [RegularExpression("abierto|en_progreso|cerrado",
        ErrorMessage = "El estado debe ser: abierto, en_progreso o cerrado")]
     public string? Status { get; set; } 
     

     public int? CategoryId {get; set; }
}