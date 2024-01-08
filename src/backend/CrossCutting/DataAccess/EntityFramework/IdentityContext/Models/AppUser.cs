using Microsoft.AspNetCore.Identity;

namespace CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;

public sealed class AppUser : IdentityUser<Guid>
{
    public string Rut { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset? LastJwtCreated { get; set; }

    public override string UserName
    {
        get => $"{Name} {FirstName} {LastName}";
        set { }
    }

    public static class RutField
    {
        public const int MaxLength = 12;
    }

    public static class NameField
    {
        public const int MaxLength = 128;
    }

    public static class FirstNameField
    {
        public const int MaxLength = 64;
    }

    public static class LastNameField
    {
        public const int MaxLength = 64;
    }

    public static class UserNameField
    {
        public const int MaxLength = NameField.MaxLength + FirstNameField.MaxLength + LastNameField.MaxLength;
    }

    public static class EmailField
    {
        public const int MaxLength = 256;
    }

    public static class PhoneNumberField
    {
        public const int MaxLength = 256;
    }
}
