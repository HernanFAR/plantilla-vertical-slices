using Microsoft.AspNetCore.Identity;

namespace CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;

public sealed class AppUser : IdentityUser<Guid>
{
    private string _name = string.Empty;
    private string _fatherLastName = string.Empty;
    private string _motherLastName = string.Empty;

    public string Rut { get; set; } = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            base.UserName = UserName;
        }
    }

    public string FatherLastName
    {
        get => _fatherLastName;
        set
        {
            _fatherLastName = value;
            base.UserName = UserName;
        }
    }

    public string MotherLastName
    {
        get => _motherLastName;
        set
        {
            _motherLastName = value;
            base.UserName = UserName;
        }
    }

    public DateTimeOffset? LastJwtCreated { get; set; }
    public DateTimeOffset? LastSessionStarted { get; set; }

    public override string UserName
    {
        get => $"{Name} {FatherLastName} {MotherLastName}";
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

    public static class FatherLastNameField
    {
        public const int MaxLength = 64;
    }

    public static class MotherLastNameField
    {
        public const int MaxLength = 64;
    }

    public static class UserNameField
    {
        public const int MaxLength = NameField.MaxLength + FatherLastNameField.MaxLength + MotherLastNameField.MaxLength;
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
