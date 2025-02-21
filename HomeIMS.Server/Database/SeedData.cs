using HomeIMS.Server.Database;
using HomeIMS.Server.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class SeedData
{
    private static readonly IEnumerable<SeedUser> seedUsers =
    [
        new SeedUser()
        {
            Email = "leela@contoso.com", 
            NormalizedEmail = "LEELA@CONTOSO.COM", 
            NormalizedUserName = "LEELA@CONTOSO.COM", 
            RoleList = [ "Administrator", "Manager" ], 
            UserName = "leela@contoso.com"
        },
        new SeedUser()
        {
            Email = "harry@contoso.com",
            NormalizedEmail = "HARRY@CONTOSO.COM",
            NormalizedUserName = "HARRY@CONTOSO.COM",
            RoleList = [ "User" ],
            UserName = "harry@contoso.com"
        },
    ];

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new IdentityContext(serviceProvider.GetRequiredService<DbContextOptions<IdentityContext>>());
        context.Database.EnsureCreated();

        if (context.Users.Any())
        {
            return;
        }

        var userStore = new UserStore<HimsUser>(context);
        var password = new PasswordHasher<HimsUser>();

        using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = [ "Administrator", "Manager", "User" ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using var userManager = serviceProvider.GetRequiredService<UserManager<HimsUser>>();

        foreach (var user in seedUsers)
        {
            var hashed = password.HashPassword(user, "Passw0rd!");
            user.PasswordHash = hashed;
            await userStore.CreateAsync(user);

            if (user.Email is not null)
            {
                var appUser = await userManager.FindByEmailAsync(user.Email);

                if (appUser is not null && user.RoleList is not null)
                {
                    await userManager.AddToRolesAsync(appUser, user.RoleList);
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private class SeedUser : HimsUser
    {
        public string[]? RoleList { get; set; }
    }
}