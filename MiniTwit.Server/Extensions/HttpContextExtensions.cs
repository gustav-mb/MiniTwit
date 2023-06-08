using System.Security.Claims;

namespace MiniTwit.Server.Extensions;

public static class HttpContextExtensions
{
    public static string? GetUserId(this HttpContext context)
    {
        if (context.User == null)
        {
            return null;
        }
        
        return context.User.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}