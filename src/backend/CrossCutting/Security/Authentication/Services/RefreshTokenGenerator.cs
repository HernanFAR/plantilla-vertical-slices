using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.Security.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CrossCutting.Security.Authentication.Services;

public class RefreshTokenGenerator(IOptions<RefreshBearerOptions> refreshBearerOptions,
    IOptions<IdentityOptions> options, 
    SystemClock systemClock)
{
    private readonly IOptions<RefreshBearerOptions> _refreshBearerOptions = refreshBearerOptions;
    private readonly IOptions<IdentityOptions> _options = options;
    private readonly SystemClock _systemClock = systemClock;

    public ValueTask<TokenInformation> GenerateAsync(AppUser user, CancellationToken _)
    {
        Claim[] claims = [new Claim(_options.Value.ClaimsIdentity.UserIdClaimType, user.Id.ToString())];

        var key = new SymmetricSecurityKey(_refreshBearerOptions.Value.KeyBytes);

        var utcTime = _systemClock.UtcNow
            .Add(_refreshBearerOptions.Value.Duration);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _refreshBearerOptions.Value.Issuer,
            audience: _refreshBearerOptions.Value.Audience,
            claims: claims,
            expires: utcTime.DateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return ValueTask.FromResult(new TokenInformation(tokenHandler.WriteToken(jwt), utcTime));
    }
}
