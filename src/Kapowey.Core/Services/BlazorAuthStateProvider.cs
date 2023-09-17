using Kapowey.Core.Services.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace Kapowey.Core.Services;
public  class BlazorAuthStateProvider : AuthenticationStateProvider
{
    private readonly AccessTokenProvider _tokenProvider;

    public BlazorAuthStateProvider(AccessTokenProvider  tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal =await _tokenProvider.GetClaimsPrincipal();
        return new AuthenticationState(claimsPrincipal);
    }
}

