using System.Collections.Concurrent;
using Kapowey.Core.Common.Interfaces.Identity;

namespace Kapowey.Core.Services;
public class UsersStateContainer : IUsersStateContainer
{
    public ConcurrentDictionary<string, string> UsersByConnectionId { get; } = new ConcurrentDictionary<string, string>();

    public event Action? OnChange;
    public void AddOrUpdate(string connectionId, string? name)
    {
        UsersByConnectionId.AddOrUpdate(connectionId, name??String.Empty, (key, oldValue) => name??String.Empty);
        NotifyStateChanged();
    }
    public void Remove(string connectionId)
    {
        UsersByConnectionId.TryRemove(connectionId, out var _);
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange?.Invoke();
}
