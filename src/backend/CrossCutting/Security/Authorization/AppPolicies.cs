using CrossCutting.Security.Authentication;
using Microsoft.AspNetCore.Authorization;

// ReSharper disable once CheckNamespace
namespace CrossCutting.Security.Authorization.Policies;

public class AppPolicies
{
    public static readonly AuthorizationPolicy DefaultPolicy =
        new AuthorizationPolicyBuilder(AuthenticationForm.Default)
            .RequireAuthenticatedUser()
            .Build();

    public static readonly AuthorizationPolicy RefreshPolicy =
        new AuthorizationPolicyBuilder(AuthenticationForm.Refresh)
            .RequireAuthenticatedUser()
            .Build();
}
