


using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration of the SMTP server used for notifications.
/// Linked from the “Email” section of appsettings.json.
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    [Required(ErrorMessage = "Email:From is required")]
    [EmailAddress(ErrorMessage = "Email:From must be a valid email address")]
    public string From { get; set;} = string.Empty;

    [Required(ErrorMessage = "Email:DisplayName is required")]
    public string DisplayName { get; set;} = string.Empty;

    [Required(ErrorMessage = "Email:Host is required")]
    public string Host { get; set;} = string.Empty;

    [Range(1, 65535, ErrorMessage = "Email:Port must be a valid port")]
    public int Port { get; set;}

    [Required(ErrorMessage = "Email:Username is required")]
    public string Username { get; set;} = string.Empty;

    [Required(ErrorMessage = "Email:Password is required")]
    public string Password { get; set;} = string.Empty;
}