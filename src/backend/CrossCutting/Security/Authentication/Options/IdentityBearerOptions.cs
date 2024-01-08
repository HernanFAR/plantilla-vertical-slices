using System.Text;

namespace CrossCutting.Security.Authentication.Options;

public class IdentityBearerOptions
{
    public const string ConfigName = nameof(IdentityBearerOptions);

    public string Key { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public TimeSpan Duration { get; init; }

    public byte[] KeyBytes => Encoding.UTF8.GetBytes(Key);
}
