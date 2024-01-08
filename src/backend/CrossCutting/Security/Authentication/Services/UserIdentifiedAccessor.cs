using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CrossCutting.Security.Authentication.Services;

public class UserIdentifiedAccessor(IHttpContextAccessor contextAccessor)
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    public string GetUserId()
    {
        var context = _contextAccessor.HttpContext;
        if (context is null) throw new InvalidOperationException(nameof(context));

        var nameIdentifierClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim is null) throw new InvalidOperationException(nameof(nameIdentifierClaim));

        return nameIdentifierClaim.Value;
    }
}
