using IncidentAPI.Api.Repositories.Interfaces;
using IncidentAPI.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Cryptography;
using System.Text;



/// <summary>
/// Controlador encargado de la autenticación de usuarios (Login, logout, reset y forgot password)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor del controlador de autenticación.
    /// </summary>
    /// <param name="userService">Servicio para validar usuarios</param>
    /// <param name="jwtService">Servicio para generar tokens JWT</param>
    /// <param name="config">Configuración de la aplicación</param>
    /// <param name="logger">Servicio de logging</param>
    public AuthController(IUserService userService, IJwtService jwtService, IEmailService emailService, IUserRepository userRepository, IConfiguration config, ILogger<AuthController> logger)
    {
        _userService = userService;
        _jwtService = jwtService;
        _emailService = emailService;
        _userRepository = userRepository;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Permite a un usuario iniciar sesión en el sistema
    /// </summary>
    /// <param name="dto">Objeto que contiene el email y contraseña del usuario</param>
    /// <returns>
    /// <response code="200">Si el login es exitoso y guarda token en una cookie</response>
    /// <response code="401">Unauthorized si las credenciales son inválidas</response>
    /// </returns>
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {   
        //Validar credenciales del usuario
        var user = await _userService.ValidateCredentialsAsync(dto.email, dto.password);

        // Si no existe el usuario o las credenciales son incorrectas
        if (user == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        // Genera un token JWT
        var token = _jwtService.GenerateToken(user);

        // Obtener tiempo de expiración desde la configuración
        var expMinutes = double.Parse(_config["Jwt:ExpirationMinutes"]!);

        // Guardar el token en una cookie segura
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,       // Evitar acceso desde JavaScript
            Secure = false,        // En local false, en producción true(HTTPS)
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(expMinutes)
        });

        return Ok(new { message = "Login exitoso" });
    }

    /// <summary>
    /// Cierra la sesión del usuario autenticado eliminando la cookie JWT
    /// </summary>
    /// <returns>
    /// <response code="200">Cuando la sesión ha sido cerrado exitosa</response>
    /// </returns>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {   
        //Eliminar la cookie que contiene el token JWT
        Response.Cookies.Delete("jwt");
        
        return Ok(new { message = "Sesión cerrada"});
    }

    /// <summary>
    /// Envía codigo de recuperación al correo del usuario si existe.
    /// </summary>
    /// <param name="dto">Contien el email del usuario</param>
    /// <returns>
    /// <response code="200">Mensaje indicando que se enviaron instrucciones</response>
    /// </returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        //Siempre devuelve Ok para no revelar si el email existe o no
        if(user == null)
            return Ok(new {message = "Si el correo existe recibirás un email con instrucciones"});

        // Generar token de 6 digitos criptográficamente seguro
        var token = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        // Guardar token y expiración
        user.ResetToken = token;
        user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
        await _userRepository.UpdateAsync(user.Id, user);

        try
        {
            // Enviar correo con el token
            await _emailService.SendPasswordResetAsync(user.Email, user.Name, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando correo de recuperación a {Email}", dto.Email);
        }

        return Ok (new {message = "Si el correo existe recibirás un email con instrucciones."});
    }

    /// <summary>
    /// Permite establecer la contraseña usando token válido
    /// </summary>
    /// <param name="dto">Contiene email, token y nueva contraseña</param>
    /// <returns>
    /// <response code="200">Mensaje indicando el resultado</response>
    /// </returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        var tokenMatches = user?.ResetToken != null && 
            CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(dto.Token),
                Encoding.UTF8.GetBytes(user.ResetToken));

        // Validar token y expiración
        if(user == null || !tokenMatches || user.ResetTokenExpiry < DateTime.UtcNow)
            return BadRequest(new {message = "Token inválido o expirado"});
        
        // Actualizar contraseña y limpiar token
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        await _userRepository.UpdateAsync(user.Id, user);

        return Ok(new {message = "Contraseña actualizada correctamente"});
    }
}