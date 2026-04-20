using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectApprovalSystem.Models;
using System;
using System.Threading.Tasks;

namespace ProjectApprovalSystem
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Data.ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                // Create roles
                string[] roles = { "Student", "Supervisor", "ModuleLeader", "SystemAdmin" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                        logger.LogInformation($"Created role: {role}");
                    }
                }

                // Create Admin user
                var adminEmail = "admin@pas.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        UserType = UserRole.SystemAdmin,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "SystemAdmin");
                        await userManager.AddToRoleAsync(admin, "ModuleLeader");
                        logger.LogInformation($"Created Admin user: {adminEmail}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Admin creation error: {error.Description}");
                        }
                    }
                }

                // Create Sample Supervisor
                var supervisorEmail = "supervisor@pas.com";
                if (await userManager.FindByEmailAsync(supervisorEmail) == null)
                {
                    var supervisor = new ApplicationUser
                    {
                        UserName = supervisorEmail,
                        Email = supervisorEmail,
                        FirstName = "John",
                        LastName = "Doe",
                        UserType = UserRole.Supervisor,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(supervisor, "Super@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(supervisor, "Supervisor");
                        logger.LogInformation($"Created Supervisor user: {supervisorEmail}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Supervisor creation error: {error.Description}");
                        }
                    }
                }

                // Create Sample Student
                var studentEmail = "student@pas.com";
                if (await userManager.FindByEmailAsync(studentEmail) == null)
                {
                    var student = new ApplicationUser
                    {
                        UserName = studentEmail,
                        Email = studentEmail,
                        FirstName = "Jane",
                        LastName = "Smith",
                        UserType = UserRole.Student,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(student, "Student@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(student, "Student");
                        logger.LogInformation($"Created Student user: {studentEmail}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Student creation error: {error.Description}");
                        }
                    }
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding error: {ex.Message}");
                throw;
            }
        }
    }
}