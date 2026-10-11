

/// <summary>
/// Reusable validation patterns for the system's DTOs
/// </summary>

public static class ValidationPatterns
{
    /// <summary>
    /// Free-form text: letters (with accents), numbers, spaces, and basic punctuation.
    /// For titles, descriptions, and comment content.
    /// </summary>
    public const string FreeText = @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.,;:!?()\-'""]+$";

    /// <summary>
    /// Person's name: letters (with accents), spaces, hyphens, and apostrophes.
    /// for the author of a comment
    /// </summary>
    public const string PersonName = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\-']+$";

    /// <summary>
    /// Category name: letters (with accents), numbers, spaces, and hyphens.
    /// </summary>
    public const string CategoryName = @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-]+$";
}