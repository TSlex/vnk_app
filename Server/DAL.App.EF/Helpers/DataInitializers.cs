using System;
using System.Collections.Generic;
using DAL.App.EF;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAL.Helpers
{
    public class DataInitializers
    {
        public static void MigrateDatabase(AppDbContext context, ILogger? logger)
        {
            logger.LogInformation("MigrateDatabase");
            context.Database.Migrate();
        }

        public static bool DeleteDatabase(AppDbContext context, ILogger? logger)
        {
            logger.LogInformation("DeleteDatabase");
            return context.Database.EnsureDeleted();
        }

        public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            ILogger? logger, IConfiguration configuration)
        {
            logger.LogInformation("SeedIdentity:AppRoles");
            var roleNames = new[]
            {
                new Role
                {
                    Name = "User",
                    LocalizedName = configuration["RolesLocalizations:UserRoleName"]
                },
                new Role
                {
                    Name = "Administrator",
                    LocalizedName = configuration["RolesLocalizations:AdministratorRoleName"]
                },
                new Role
                {
                    Name = "Root",
                    LocalizedName = configuration["RolesLocalizations:RootRoleName"]
                }
            };

            foreach (var roleName in roleNames)
            {
                var role = roleManager.FindByNameAsync(roleName.Name).Result;

                if (role == null)
                {
                    role = new AppRole {Name = roleName.Name, LocalizedName = roleName.LocalizedName};

                    var result = roleManager.CreateAsync(role).Result;

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException("Role creation failed!");
                    }
                }
            }

            logger.LogInformation("SeedIdentity:AppUsers");
            var users = new[]
            {
                new User
                {
                    FirstName = "Александр",
                    LastName = "Иванов",
                    Email = "admin@admin.com",
                    Password = "Admin_123",
                    RolesNames = new[]
                    {
                        "Administrator"
                    }
                },
                new User
                {
                    FirstName = configuration["SuperUser:FirstName"],
                    LastName = configuration["SuperUser:LastName"],
                    Email = configuration["SuperUser:Email"],
                    Password = configuration["SuperUser:Password"],
                    RolesNames = new[]
                    {
                        "Root"
                    }
                }
            };

            foreach (var user in users)
            {
                var newUser = userManager.FindByEmailAsync(user.Email).Result;
                if (newUser == null)
                {
                    newUser = new AppUser()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.Email,
                        EmailConfirmed = true,
                    };

                    var result = userManager.CreateAsync(newUser, user.Password).Result;
                    if (!result.Succeeded)
                    {
                        throw new ApplicationException("User creation failed!");
                    }

                    foreach (var roleName in user.RolesNames!)
                    {
                        var roleResult = userManager.AddToRoleAsync(newUser, roleName).Result;

                        if (!roleResult.Succeeded)
                        {
                            throw new ApplicationException("User role assigment failed!");
                        }
                    }
                }
                else
                {
                    var resetToken = userManager.GeneratePasswordResetTokenAsync(newUser).Result;
                    var changePasswordResult =
                        userManager.ResetPasswordAsync(newUser, resetToken, user.Password).Result;
                    if (!changePasswordResult.Succeeded)
                    {
                        throw new ApplicationException("Passwords resetting failed!");
                    }
                }
            }
        }

        public static void SeedData(AppDbContext ctx, ILogger? logger)
        {
            logger.LogInformation("SeedData");
        }

        private struct User
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

            public ICollection<string>? RolesNames { get; set; }
        }

        private struct Role
        {
            public string Name { get; set; }
            public string LocalizedName { get; set; }
        }
    }
}