using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.Security.Authentication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CrossCutting.DataAccess.Identity;

public class ApplicationClaimsFactory(
    ApplicationUserManager userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<AppUser>(userManager, optionsAccessor)
{
}
