using System;
using System.Collections.Generic;
using DAL.App.EF;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            ILogger? logger)
        {
            logger.LogInformation("SeedIdentity:AppRoles");
            var roleNames = new[]
            {
                new Role
                {
                    Name = "Authorised"
                },
                new Role
                {
                    Name = "Super Admin"
                }
            };

            foreach (var roleName in roleNames)
            {
                var role = roleManager.FindByNameAsync(roleName.Name).Result;

                if (role == null)
                {
                    role = new AppRole() {Name = roleName.Name};

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
                    UserName = "admin",
                    Email = "admin@admin.com",
                    Password = "Admin_123",
                    RolesNames = new[]
                    {
                        "Authorised"
                    }
                },
                new User
                {
                    UserName = "root",
                    Email = "root@root.com",
                    Password = "Admin_123",
                    RolesNames = new[]
                    {
                        "Authorised", "Super Admin"
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
                        Email = user.Email,
                        UserName = user.UserName,
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
            public long Id { get; set; }

            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            public ICollection<string>? RolesNames { get; set; }
        }

        private struct Role
        {
            public long Id { get; set; }

            public string Name { get; set; }
        }
    }
}