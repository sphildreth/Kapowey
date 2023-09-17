using System.Security.Claims;
using Kapowey.Core.Common.Interfaces.Identity.DTOs;
using Kapowey.Core.Common.Models;
using Kapowey.Core.Entities;

namespace Kapowey.Core.Common.Interfaces.Identity;

public interface IIdentityService : IService
{
    Task<Result<TokenResponse>> LoginAsync(TokenRequest request, CancellationToken cancellation = default);
    Task<string> GenerateJwtAsync(User user);
    Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation = default);
    Task<ClaimsPrincipal> GetClaimsPrincipal(string token);
    Task<string?> GetUserNameAsync(int userId, CancellationToken cancellation = default);
    
    Task<bool> IsInRoleAsync(int userId, string role, CancellationToken cancellation = default);
    Task<bool> AuthorizeAsync(int userId, string policyName, CancellationToken cancellation = default);
    Task<Result> DeleteUserAsync(int userId, CancellationToken cancellation = default);
    Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default);
    Task UpdateLiveStatus(int userId, bool isLive, CancellationToken cancellation = default);
    Task<Models.API.Entities.User> GetApplicationUserDto(int userId,CancellationToken cancellation=default);
    string GetUserName(int userId);
    Task<List<Models.API.Entities.User>?> GetUsers(string? tenantId, CancellationToken cancellation = default);
}