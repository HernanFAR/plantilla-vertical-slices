using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.DataAccess.Identity;
using CrossCutting.Security.Authentication.Options;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CrossCutting.Security.Authentication.Services;

public class JwtTokenGenerator(IOptions<IdentityBearerOptions> identityBearerOptions, 
    AppSignInManager signInManager,
    SystemClock systemClock)
{
    private readonly IOptions<IdentityBearerOptions> _identityBearerOptions = identityBearerOptions;
    private readonly AppSignInManager _signInManager = signInManager;
    private readonly SystemClock _systemClock = systemClock;

    public async ValueTask<TokenInformation> GenerateAsync(AppUser user, CancellationToken _)
    {
        var identity = await _signInManager.CreateUserPrincipalAsync(user);
        var key = new SymmetricSecurityKey(_identityBearerOptions.Value.KeyBytes);

        var utcExpire = _systemClock.UtcNow
            .Add(_identityBearerOptions.Value.Duration);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _identityBearerOptions.Value.Issuer,
            audience: _identityBearerOptions.Value.Audience,
            claims: identity.Claims,
            expires: utcExpire.DateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new TokenInformation(tokenHandler.WriteToken(jwt), utcExpire);
    }
}
