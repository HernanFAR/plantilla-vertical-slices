﻿using CrossCutting.DataAccess.EntityFramework;
using CrossCutting.DataAccess.EntityFramework.IdentityContext.Models;
using CrossCutting.DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class DataAccessConfiguration
{
    public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services)
    {
        return services
            .AddDbContext<AppDbContext>(ConfigureDatabase)
            .AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "aábcdeéfghiíjklmnñoópqrstuúvwxyzAÁBCDEÉFGHIÍJKLMNÑOÓPQRSTUÚVWXYZ ";

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserManager<AppUserManager>()
            .AddSignInManager<AppSignInManager>()
            .AddClaimsPrincipalFactory<AppClaimsFactory>()
            .Services;
    }

    public static void ConfigureDatabase(DbContextOptionsBuilder b)
    {
        b.UseSqlServer("Name=ConnectionStrings:Database");
    }
}
