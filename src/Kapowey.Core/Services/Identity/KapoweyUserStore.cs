using Kapowey.Core.Entities;
using Kapowey.Core.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Kapowey.Core.Services.Identity;

public class KapoweyUserStore : UserStore<User, UserRole, KapoweyContext, int, UserClaim, UserUserRole, UserLogin, UserToken, UserRoleClaim>
{
    public KapoweyUserStore(KapoweyContext context) 
        : base(context) 
    { 
    }

    public override Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken = new CancellationToken())
    {
        return base.AddLoginAsync(user, login, cancellationToken);
    }

    protected override UserLogin CreateUserLogin(User user, UserLoginInfo login)
    {
        return new UserLogin
        {
            UserId = user.Id,
            ProviderKey = login.ProviderKey,
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName
        };
    }
}