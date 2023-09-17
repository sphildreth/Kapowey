using System.Security.Claims;
using System.Security.Cryptography;
using Kapowey.Core.Common.Extensions;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Interfaces.Identity;
using Kapowey.Core.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Kapowey.Core.Services.Identity;
public class AccessTokenProvider
{
    private readonly string _tokenKey = nameof(_tokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;
    public string? AccessToken { get; private set; }

    public AccessTokenProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService,ICurrentUserService currentUser)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _identityService = identityService;
        _currentUser = currentUser;
    }
    public async Task GenerateJwt(User user)
    {
        AccessToken = await _identityService.GenerateJwtAsync(user);
        await _localStorage.SetAsync(_tokenKey, AccessToken);
        _currentUser.UserId = user.Id;
        _currentUser.UserName = user.UserName;
    }
    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            var token = await _localStorage.GetAsync<string>(_tokenKey);
            if (token.Success && !string.IsNullOrEmpty(token.Value))
            {
                AccessToken = token.Value;
                var principal = await _identityService.GetClaimsPrincipal(token.Value);
                if (principal?.Identity?.IsAuthenticated ?? false)
                {
                    _currentUser.UserId = principal?.GetUserId() ?? 0;
                    _currentUser.UserName = principal?.GetUserName();
                    return principal!;
                }
            }
        }
        catch (CryptographicException)
        {
            await RemoveAuthDataFromStorage();
        }
        catch (Exception)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }


    public async Task RemoveAuthDataFromStorage()
    {
        await _localStorage.DeleteAsync(_tokenKey);
        _navigation.NavigateTo("/", true);
    }
}
