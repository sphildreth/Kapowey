using System.Security.Claims;
using Kapowey.Core.Common.Constants;
using Kapowey.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Kapowey.Core.Services;
#nullable disable
public class ApplicationClaimsIdentityFactory : UserClaimsPrincipalFactory<User, UserRole>
{
    private readonly UserManager<User> _userManager;

    public ApplicationClaimsIdentityFactory(UserManager<User> userManager,
        RoleManager<UserRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
    }
    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var principal = await base.CreateAsync(user);
        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            });
        }
        if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
        {
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl)
            });
        }
        var appuser = await _userManager.FindByIdAsync(user.Id.ToString());
        var roles = await _userManager.GetRolesAsync(appuser);
        if (roles != null && roles.Count > 0)
        {
            var rolesStr = string.Join(",", roles);
            ((ClaimsIdentity)principal.Identity)?.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.AssignedRoles, rolesStr)
            });
        }
        return principal;
    }
}
