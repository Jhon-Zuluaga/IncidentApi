


using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration for generating and validating JWT tokens.
/// Linked from the “Jwt” section of appsettings.json.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Jwt:Key is required")]
    [MinLength(32, ErrorMessage = "Jwt:The key must be at least 32 characters long")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jwt:Issuer is required")]
    public string Issuer { get; set;} = string.Empty;

    [Required(ErrorMessage = "Jwt:Audience is required")]
    public string Audience { get; set;} = string.Empty;

    [Range(1, 1440, ErrorMessage = "Jwt:ExpirationMinutes must be between 1 and 1440")]
    public double ExpirationMinutes { get; set;} = 60;
}