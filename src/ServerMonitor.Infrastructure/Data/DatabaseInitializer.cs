using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerMonitor.Domain;

namespace ServerMonitor.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scoped = scope.ServiceProvider;

        await scoped.GetRequiredService<AppDbContext>().Database.MigrateAsync();

        var roleManager = scoped.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var configuration = scoped.GetRequiredService<IConfiguration>();
        var email = configuration["Seed:AdminEmail"];
        var password = configuration["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var userManager = scoped.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);
    }
}
