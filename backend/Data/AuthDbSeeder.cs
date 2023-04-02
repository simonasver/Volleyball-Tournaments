using Backend.Auth.Model;
using Microsoft.AspNetCore.Identity;

namespace Backend.Data;

public class AuthDbSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AuthDbSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await AddDefaultRoles();
        await AddAdminUser();
    }

    private async Task AddDefaultRoles()
    {
        foreach (var role in ApplicationUserRoles.All)
        {
            var roleExists = await _roleManager.RoleExistsAsync((role));
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task AddAdminUser()
    {
        var newAdminUser = new ApplicationUser()
        {
            UserName = "admin",
            FullName = "Admin admin",
            Email = "admin@admin.com",
            RegisterDate = DateTime.Now,
        };

        var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existingAdminUser == null)
        {
            var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "123Admin123!");
            if (createAdminUserResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(newAdminUser, ApplicationUserRoles.All);
            }
        }
    }
}
