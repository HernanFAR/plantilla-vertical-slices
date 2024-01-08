namespace CrossCutting.Security.Authentication;

public static class AuthenticationForm
{
    public const string Default = "Default";
    public const string Refresh = "Refresh";
}

public record struct TokenInformation(string Value, DateTimeOffset Expire);
