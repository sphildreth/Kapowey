using Kapowey.Core.Common.Interfaces.Identity;
using Kapowey.Core.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kapowey.Core.Services.Identity;
public class UserDataProvider : IUserDataProvider
{
    private readonly UserManager<User> _userManager;
    public List<Common.Models.API.Entities.User> DataSource { get; private set; }

    public event Action? OnChange;

    public UserDataProvider(
        IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        DataSource = new List<Common.Models.API.Entities.User>();
    }
    public void Initialize()
    {
        DataSource = _userManager.Users.Include(x => x.UserUserRole).ThenInclude(x => x.UserRole).ProjectToType<Common.Models.API.Entities.User>().OrderBy(x=>x.UserName).ToList();
        OnChange?.Invoke();
    }

    public async Task InitializeAsync()
    {
        DataSource =await _userManager.Users.Include(x => x.UserUserRole).ThenInclude(x => x.UserRole).ProjectToType<Common.Models.API.Entities.User>().OrderBy(x => x.UserName).ToListAsync();
        OnChange?.Invoke();
    }

    public Task Refresh()
    {
        OnChange?.Invoke();
        return Task.CompletedTask;
    }
}
