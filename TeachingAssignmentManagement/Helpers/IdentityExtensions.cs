using System.Security.Claims;
using System.Security.Principal;

namespace TeachingAssignmentManagement.Helpers
{
    public static class IdentityExtensions
    {
        public static string GetRole(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Role);

            return claim?.Value ?? string.Empty;
        }
    }
}