using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.Security.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CrossCutting.Security.Authentication.Services;

public class RefreshTokenGenerator(RefreshBearerOptions refreshBearerOptions,
    IOptions<IdentityOptions> options, 
    SystemClock systemClock)
{
    private readonly RefreshBearerOptions _refreshBearerOptions = refreshBearerOptions;
    private readonly IOptions<IdentityOptions> _options = options;
    private readonly SystemClock _systemClock = systemClock;

    public ValueTask<TokenInformation> GenerateAsync(AppUser user, CancellationToken _)
    {
        Claim[] claims = [new Claim(_options.Value.ClaimsIdentity.UserIdClaimType, user.Id.ToString())];

        var key = new SymmetricSecurityKey(_refreshBearerOptions.KeyBytes);

        var utcTime = _systemClock.UtcNow
            .Add(_refreshBearerOptions.Duration);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _refreshBearerOptions.Issuer,
            audience: _refreshBearerOptions.Audience,
            claims: claims,
            expires: utcTime.DateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return ValueTask.FromResult(new TokenInformation(tokenHandler.WriteToken(jwt), utcTime));
    }
}
