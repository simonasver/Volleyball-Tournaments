using Backend.Data.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Backend.Data;

public class AuthDbSeeder
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public AuthDbSeeder(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
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
            UserName = _configuration["DefaultAdminUser:Username"],
            FullName = _configuration["DefaultAdminUser:Username"],
            Email = _configuration["DefaultAdminUser:Email"],
            RegisterDate = DateTime.Now,
        };

        if (string.IsNullOrEmpty(newAdminUser.UserName))
        {
            return;
        }

        var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existingAdminUser == null)
        {
            var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, _configuration["DefaultAdminUser:Password"]);
            if (createAdminUserResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(newAdminUser, ApplicationUserRoles.All);
            }
        }
    }
}
