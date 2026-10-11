using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Api.DTOs.Comment;

/*
    Se usa en Updates (PUT/PATCH)
    Permite modificar los datos del comentario con validaciones
*/

// DTO para actualizar un comentario existente
public class UpdateCommentDto
{
    [MinLength(3, ErrorMessage = "El contenido debe tener al menos 3 caracteres")]
    [MaxLength(500, ErrorMessage = "El contenido no puede tener más de 500 caracteres")]
    [RegularExpression(ValidationPatterns.FreeText,
        ErrorMessage = "El contenido solo puede contener letras")]
    public string? Content { get; set; }

    
     [MinLength(3, ErrorMessage = "El author debe tener al menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "El author no puede tener más de 100 caracteres")]
    [RegularExpression(ValidationPatterns.PersonName,
        ErrorMessage = "El author solo puede contener letras")]
    public string? Author { get; set; }
}