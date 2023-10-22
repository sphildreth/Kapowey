using Kapowey.Core.Entities;
using Kapowey.Core.Persistance;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kapowey.Core.Services.Identity;

public class KapoweySignInManager : SignInManager<User>
{
    private readonly IDbContextFactory<KapoweyContext> _contextFactory;

    public KapoweySignInManager(KapoweyUserManager userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<KapoweySignInManager> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation,
        IDbContextFactory<KapoweyContext> contextFactory) 
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _contextFactory = contextFactory;
    }

    // public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
    // {
    //     using (var context = _contextFactory.CreateDbContext())
    //     {
    //         var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
    //         if (user == null)
    //         {
    //             return SignInResult.Failed;
    //         }
    //         
    //         var loginInfo = await GetExternalLoginInfoAsync();
    //         if (loginInfo == null)
    //         {
    //             return SignInResult.Failed;
    //         }
    //         var userLoginForProviderKey = await context.UserLogins.FirstOrDefaultAsync(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
    //         if (userLoginForProviderKey != null)
    //         {
    //             
    //         }
    //     }
    //
    //     //return base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
    //     throw new NotImplementedException();
    // }
    //
    // public override Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
    //     => ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, false);
}