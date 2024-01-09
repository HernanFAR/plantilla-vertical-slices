using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrossCutting.DataAccess.EntityFramework.IdentityContext.Relations;

internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Rut)
            .HasMaxLength(AppUser.RutField.MaxLength)
            .IsRequired();

        builder.Property(u => u.Name)
            .HasMaxLength(AppUser.NameField.MaxLength)
            .IsRequired();

        builder.Property(u => u.FatherLastName)
            .HasMaxLength(AppUser.FatherLastNameField.MaxLength)
            .IsRequired();

        builder.Property(u => u.MotherLastName)
            .HasMaxLength(AppUser.MotherLastNameField.MaxLength)
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasMaxLength(AppUser.UserNameField.MaxLength)
            .IsRequired();

        builder.Property(u => u.NormalizedUserName)
            .HasMaxLength(AppUser.UserNameField.MaxLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(AppUser.EmailField.MaxLength)
            .IsRequired();

        builder.Property(u => u.NormalizedEmail)
            .HasMaxLength(AppUser.EmailField.MaxLength)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(AppUser.PhoneNumberField.MaxLength)
            .IsRequired(false);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

    }
}
