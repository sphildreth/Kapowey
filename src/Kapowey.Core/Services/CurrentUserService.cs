using Kapowey.Core.Common.Interfaces;

namespace Kapowey.Core.Services;

public class CurrentUserService : ICurrentUserService
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
}
