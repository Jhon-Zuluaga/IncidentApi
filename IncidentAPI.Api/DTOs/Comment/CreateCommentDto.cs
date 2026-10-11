using System.ComponentModel.DataAnnotations;

namespace IncidentAPI.Api.DTOs.Comment;

/*
    Se usa cuando el cliente crea un comentario
    Incluye validaciones para asegurar datos correctos
*/

// DTO para crear un nuevo comentario
public class CreateCommentDto
{
    [Required]
    [MinLength(3, ErrorMessage = "El contenido debe tener al menos 3 caracteres")]
    [MaxLength(500, ErrorMessage = "El contenido no puede tener más de 500 caracteres")]
    [RegularExpression(ValidationPatterns.FreeText,
        ErrorMessage = "El contenido contiene caracteres no permitidos")]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MinLength(3, ErrorMessage = "El autor debe tener al menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "El autor no puede tener más de 100 caracteres")]
    [RegularExpression(ValidationPatterns.PersonName,
        ErrorMessage = "El autor solo puede contener letras")]
    public string Author { get; set; } = string.Empty;

    [Required]
    public int IncidentId { get; set; }
}