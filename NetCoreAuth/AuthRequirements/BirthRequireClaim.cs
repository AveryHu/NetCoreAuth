using Microsoft.AspNetCore.Authorization;

namespace NetCoreAuth.AuthRequirements
{
    public class BirthRequireClaim : IAuthorizationRequirement
    {
        public BirthRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; set; }
    }

    public class BirthRequireClaimHandler : AuthorizationHandler<BirthRequireClaim>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BirthRequireClaim requirement)
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomClaim(
            this AuthorizationPolicyBuilder builder,
            string claimType)
        {
            builder.AddRequirements(new BirthRequireClaim(claimType));
            return builder;
        }
    }
}