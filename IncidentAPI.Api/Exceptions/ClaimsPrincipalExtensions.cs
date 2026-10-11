

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the ID of the authenticated user from the "sub" claim in the JWT.
    /// </summary>
    
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (id is null || !int.TryParse(id, out var userId))
            throw new ForbiddenException("Invalid Token: Does not contain an identifiable user.");
        return userId;
    }
}