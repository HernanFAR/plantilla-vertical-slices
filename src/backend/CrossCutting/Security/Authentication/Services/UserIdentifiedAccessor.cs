using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CrossCutting.Security.Authentication.Services;

public class UserIdentifiedAccessor(
    IHttpContextAccessor contextAccessor,
    IOptions<IdentityOptions> options)
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly IOptions<IdentityOptions> _options = options;

    public string GetUserId()
    {
        var context = _contextAccessor.HttpContext;
        if (context is null) throw new InvalidOperationException(nameof(context));

        var nameIdentifierClaim = context.User.FindFirst(_options.Value.ClaimsIdentity.UserIdClaimType);
        if (nameIdentifierClaim is null) throw new InvalidOperationException(nameof(nameIdentifierClaim));

        return nameIdentifierClaim.Value;
    }

    public DateTimeOffset? GetIssuedTime()
    {
        var context = _contextAccessor.HttpContext;
        if (context is null) throw new InvalidOperationException(nameof(context));

        var nameIdentifierClaim = context.User.FindFirst(ClaimTypes.AuthenticationInstant);
        if (nameIdentifierClaim is null) throw new InvalidOperationException(nameof(nameIdentifierClaim));

        _ = long.TryParse(nameIdentifierClaim.Value, out var issuedTime);

        if (issuedTime == 0) return null;

        return new DateTimeOffset(issuedTime, TimeSpan.Zero);
    }
}
