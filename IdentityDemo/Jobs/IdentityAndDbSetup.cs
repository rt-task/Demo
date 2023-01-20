using IdentityDemo.Dal;
using IdentityDemo.Dal.Entities;
using IdentityDemo.Identity.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class IdentityAndDbSetup : IHostedService
{
    private readonly IdentityUser<int> _adminInfo;
    private readonly List<string> _roles;
    private readonly IServiceProvider _services;

    public IdentityAndDbSetup(
        IOptions<IdentityUser<int>> adminInfo,
        IOptions<List<string>> roles,
        IServiceProvider services)
    {
        _adminInfo = adminInfo.Value;
        _roles = roles.Value;
        _services = services;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var appCtx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var authCtx = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<int>>>();

        await appCtx.Database.MigrateAsync();
        await authCtx.Database.MigrateAsync();
        await EnsureCreateRoles(roleManager);
        await EnsureCreateAdmin(userManager, authCtx, appCtx);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsureCreateRoles(RoleManager<IdentityRole<int>> roleManager)
    {
        foreach (var rolename in _roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(rolename);
            if (!roleExist)
                await roleManager.CreateAsync(new IdentityRole<int> { Name = rolename });
        }
    }

    private async Task EnsureCreateAdmin(UserManager<IdentityUser<int>> userManager, AuthDbContext authCtx, AppDbContext appCtx)
    {
        var companyToAdd = new CompanyEntity { Name = "Admin company", Description = "Admin company", Email = _adminInfo.Email, Image = "admin.png" };

        using var outerTransaction = await appCtx.Database.BeginTransactionAsync();
        using var innerTransaction = await authCtx.Database.BeginTransactionAsync();
        try
        {
            var company = await appCtx.Companies.FirstOrDefaultAsync(c => c.Email == _adminInfo.Email);
            if (company is null)
            {
                var companyResult = await appCtx.Companies.Add(companyToAdd).Context.SaveChangesAsync();
                if (companyResult <= 0)
                {
                    await innerTransaction.RollbackAsync();
                    await outerTransaction.RollbackAsync();
                    return;
                }
            }
            
            var user = await userManager.FindByEmailAsync(_adminInfo.Email);
            if (user is null)
            {
                var authSignUpResult = await userManager.CreateAsync(_adminInfo, "AAAaaa1!");
                var addToRoleResult = IdentityResult.Failed();
                if (authSignUpResult.Succeeded)
                    addToRoleResult = await userManager.AddToRoleAsync(_adminInfo, "Admin");

                if (!addToRoleResult.Succeeded)
                {
                    await innerTransaction.RollbackAsync();
                    await outerTransaction.RollbackAsync();
                    return;
                }
            }

            await innerTransaction.CommitAsync();
            await outerTransaction.CommitAsync();
            return;
        }
        catch
        {
            await innerTransaction.RollbackAsync();
            await outerTransaction.RollbackAsync();
            throw;
        }
    }
}