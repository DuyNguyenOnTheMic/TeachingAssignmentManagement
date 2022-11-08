using System.Security.Claims;
using System.Security.Principal;

namespace TeachingAssignmentManagement.Helpers
{
    public static class IdentityExtensions
    {
        public static string GetRole(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Role);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}