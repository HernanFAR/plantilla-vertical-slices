using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.DataAccess.Identity;
using CrossCutting.Security.Authentication.Options;
using Microsoft.Extensions.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CrossCutting.Security.Authentication.Services;

public class JwtTokenGenerator(IdentityBearerOptions identityBearerOptions, 
    ApplicationSignInManager signInManager,
    SystemClock systemClock)
{
    private readonly IdentityBearerOptions _identityBearerOptions = identityBearerOptions;
    private readonly ApplicationSignInManager _signInManager = signInManager;
    private readonly SystemClock _systemClock = systemClock;

    public async ValueTask<TokenInformation> GenerateAsync(AppUser user, CancellationToken _)
    {
        var identity = await _signInManager.CreateUserPrincipalAsync(user);
        var key = new SymmetricSecurityKey(_identityBearerOptions.KeyBytes);

        var utcExpire = _systemClock.UtcNow
            .Add(_identityBearerOptions.Duration);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _identityBearerOptions.Issuer,
            audience: _identityBearerOptions.Audience,
            claims: identity.Claims,
            expires: utcExpire.DateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new TokenInformation(tokenHandler.WriteToken(jwt), utcExpire);
    }
}
