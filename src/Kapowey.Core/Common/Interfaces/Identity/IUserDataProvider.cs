

namespace Kapowey.Core.Common.Interfaces.Identity;
public interface IUserDataProvider
{
    List<Models.API.Entities.User> DataSource { get; }
    event Action? OnChange;
    Task InitializeAsync();
    void Initialize();
    Task Refresh();
}
