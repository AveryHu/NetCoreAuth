using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NetCoreAuth.PolicyProvider
{
    public class SecurityLevel : AuthorizeAttribute
    {
        public SecurityLevel(int level)
        {
            Policy = $"{DynamicPolicies.SecurityLevel}.{level}";
        }
    }

    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }

        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";
    }

    public static class DynamicAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts.First();
            var value = parts.Last();

            switch (type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim("Rank", value)
                        .Build();

                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value)))
                        .Build();

                default:
                    return null;
            }
        }
    }

    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; }

        public SecurityLevelRequirement(int level)
        {
            Level = level;
        }
    }

    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SecurityLevelRequirement requirement)
        {
            var claimValue = Convert.ToInt32(context.User.Claims
                .FirstOrDefault(x => x.Type == DynamicPolicies.SecurityLevel)
                ?.Value ?? "0");

            if (requirement.Level <= claimValue)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }

    public class MyAuthorizationPolicyProvider
        : DefaultAuthorizationPolicyProvider
    {
        public MyAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var dynamicPolicy in DynamicPolicies.Get())
            {
                if (policyName.StartsWith(dynamicPolicy))
                {
                    var policy = DynamicAuthorizationPolicyFactory.Create(policyName);

                    return Task.FromResult(policy);
                }
            }
            return base.GetPolicyAsync(policyName);
        }
    }
}