using System;
using System.Collections.Generic;
using System.Linq;
using DAL.App.Entities;
using DAL.App.Entities.Enums;
using DAL.App.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DAL.App.EF.Helpers
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
                if (user.RolesNames!.Contains("Root") && userManager.GetUsersInRoleAsync("Root").Result.Any())
                {
                    continue;
                }
                
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

        public static void SeedData(AppDbContext context, ILogger? logger)
        {
            logger.LogInformation("SeedData");

            var types = new List<AttributeType>()
            {
                new()
                {
                    Name = "Строка",
                    DataType = AttributeDataType.String,
                    SystemicType = true,
                    DefaultCustomValue = ""
                },
                new()
                {
                    Name = "Тождество",
                    DataType = AttributeDataType.Boolean,
                    SystemicType = true,
                    DefaultCustomValue = "false"
                },
                new()
                {
                    Name = "Целое число",
                    DataType = AttributeDataType.Integer,
                    SystemicType = true,
                    DefaultCustomValue = "0"
                },
                new()
                {
                    Name = "Число с плавающей точкой",
                    DataType = AttributeDataType.Float,
                    SystemicType = true,
                    DefaultCustomValue = "0.00"
                },
                new()
                {
                    Name = "Дата",
                    DataType = AttributeDataType.Date,
                    SystemicType = true,
                    DefaultCustomValue = "2021.01.01"
                },
                new()
                {
                    Name = "Время",
                    DataType = AttributeDataType.Time,
                    SystemicType = true,
                    DefaultCustomValue = "12:00"
                },
                new()
                {
                    Name = "Дата со временем",
                    DataType = AttributeDataType.DateTime,
                    SystemicType = true,
                    DefaultCustomValue = "2021.01.01 12:00"
                },
            };

            foreach (var type in types)
            {
                context.AttributeTypes.Add(type);
            }

            context.SaveChanges();
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