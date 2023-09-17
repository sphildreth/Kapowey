namespace Kapowey.Core.Common.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; set; }
    string? UserName { get; set; }
}