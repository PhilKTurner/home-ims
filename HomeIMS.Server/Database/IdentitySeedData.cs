using HomeIMS.Server.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeIMS.Server.Database;

public class IdentitySeedData
{
    private static readonly SeedUser rootUser = new SeedUser
    {
        UserName = "root",
        NormalizedUserName = "ROOT",
        RoleList = [ "Administrator" ]
    };

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

        string[] roles = [ "Administrator" ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var hashedRootPassword = password.HashPassword(rootUser, configuration["hims-root-pw"] ?? "");
        rootUser.PasswordHash = hashedRootPassword;

        using var userManager = serviceProvider.GetRequiredService<UserManager<HimsUser>>();
        await InitializeUser(userStore, userManager, rootUser);

        await context.SaveChangesAsync();
    }

    private static async Task<object> InitializeUser(UserStore<HimsUser> userStore, UserManager<HimsUser> userManager, SeedUser user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName))
            throw new ArgumentException("User name is required", nameof(user.UserName));

        await userStore.CreateAsync(user);

        var appUser = await userManager.FindByNameAsync(user.UserName);

        if (appUser is not null && user.RoleList is not null)
        {
            await userManager.AddToRolesAsync(appUser, user.RoleList);
        }

        return user;
    }


    private class SeedUser : HimsUser
    {
        public string[]? RoleList { get; set; }
    }
}