using System.Text;

namespace CrossCutting.Security.Authentication.Options;

public class RefreshBearerOptions
{
    public const string ConfigName = nameof(RefreshBearerOptions);

    public string Key { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public TimeSpan Duration { get; init; }

    public byte[] KeyBytes => Encoding.UTF8.GetBytes(Key);
}
