
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IncidentAPI.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// Servicio que implementa la generación tokens JWT
public class JwtService(IOptions<JwtOptions> jwtOptions) : IJwtService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken(User user)
    {
        // Obtener clave secreta desde configuración
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        // Credenciales para firmar token
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Informacion se guarda dentro del token (claims)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Tiempo expiracion del token
        var expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        // Crear token JWT con toda la informacion
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        // Convertir el token a string para enviarlo
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Flujo:  user -> claims -> se firma -> se crea token -> se devuelve string