
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KT9_AdminPanel.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        bool isDevelopment)
    {
        using var scope = services.CreateScope();

        var provider = scope.ServiceProvider;

        var db = provider.GetRequiredService<ApplicationDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();

        await db.Database.MigrateAsync();

        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                EnsureSuccess(result, $"Создание роли {role}");
            }
        }

        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                admin, adminPassword);

            EnsureSuccess(result, "Создание администратора");
        }
        else
        {
            if (!admin.EmailConfirmed)
            {
                admin.EmailConfirmed = true;
                var result = await userManager.UpdateAsync(admin);
                EnsureSuccess(result, "Подтверждение email");
            }

            // Сброс пароля только для демонстрационного аккаунта
            // при разработке учебного проекта.
            if (isDevelopment &&
                !await userManager.CheckPasswordAsync(admin, adminPassword))
            {
                var token =
                    await userManager.GeneratePasswordResetTokenAsync(admin);

                var result = await userManager.ResetPasswordAsync(
                    admin, token, adminPassword);

                EnsureSuccess(result, "Сброс пароля администратора");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Admin");
            EnsureSuccess(result, "Назначение роли Admin");
        }

        Console.WriteLine("Identity seeding completed.");
        Console.WriteLine($"Administrator: {adminEmail}");
    }

    private static void EnsureSuccess(
        IdentityResult result,
        string operation)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"{operation}: {errors}");
        }
    }
}
