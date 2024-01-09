using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CrossCutting.DataAccess.Identity;

public class AppClaimsFactory(
    AppUserManager userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<AppUser>(userManager, optionsAccessor)
{
    public new AppUserManager UserManager { get; } = userManager;
}
