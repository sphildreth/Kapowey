using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.ExceptionHandlers;
using Kapowey.Core.Common.Extensions;
using Kapowey.Core.Common.Interfaces.Identity;
using Kapowey.Core.Common.Interfaces.Identity.DTOs;
using Kapowey.Core.Common.Models;
using Kapowey.Core.Entities;
using LazyCache;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace Kapowey.Core.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<UserRole> _roleManager;
    private readonly AppConfigurationSettings _appConfig;
    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAppCache _cache;
    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);
    private LazyCacheEntryOptions Options => new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);
    public IdentityService(
        IServiceScopeFactory scopeFactory,
        AppConfigurationSettings appConfig,
        IAppCache cache)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<User>>();
        _authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        _appConfig = appConfig;
        _cache = cache;
    }

    public async Task<string?> GetUserNameAsync(int userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), Options);
        return user?.UserName;
    }
    public string GetUserName(int userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = _cache.GetOrAdd(key, () => _userManager.Users.SingleOrDefault(u => u.Id == userId), Options);
        return user?.UserName??string.Empty;
    }
    public async Task<bool> IsInRoleAsync(int userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException("User Not Found.");
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(int userId, string policyName, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException("User Not Found.");
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;

    }

    public async Task<Result> DeleteUserAsync(int userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException("User Not Found.");
        var result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }
    public async Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default)
    {
        var result = await _userManager.Users
             .Where(x => x.UserRoles.Any(y => y.UserRole.Name == roleName))
             .Include(x => x.UserUserRole)
             .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest request, CancellationToken cancellation = default)
    {
        var user = await _userManager.FindByNameAsync(request.UserName!);
        if (user == null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { "User Not Found." });
        }
        if (!user.IsActive)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { "User Not Active. Please contact the administrator." });
        }
        if (!(user.EmailConfirmed ?? false))
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { "E-Mail not confirmed." });
        }
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
        if (!passwordValid)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { "Invalid Credentials." });
        }
        user.RefreshToken = GenerateRefreshToken();
        var tokenExpiryTime = DateTime.Now.AddDays(7);

        if (request.RememberMe)
        {
            tokenExpiryTime = DateTime.Now.AddYears(1);
        }
        user.RefreshTokenExpiryTime = tokenExpiryTime;
        await _userManager.UpdateAsync(user);
        
        var token = await GenerateJwtAsync(user);
        var response = new TokenResponse { Token = token, RefreshTokenExpiryTime = tokenExpiryTime, RefreshToken = user.RefreshToken, ProfilePictureDataUrl = user.ProfilePictureDataUrl };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation = default)
    {
        if (request is null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { "Invalid Client Token." });
        }
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email)!;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return await Result<TokenResponse>.FailureAsync(new string[] { "User Not Found." });
        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return await Result<TokenResponse>.FailureAsync(new string[] { "Invalid Client Token." });
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var token = GenerateEncryptedToken(GetSigningCredentials(), principal.Claims);
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<TokenResponse>.SuccessAsync(response);
    }
    public async Task<ClaimsPrincipal> GetClaimsPrincipal(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer =false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var result =await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        if (result.IsValid)
        {
           return new ClaimsPrincipal(result.ClaimsIdentity);
        }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    public async Task<string> GenerateJwtAsync(User user)
    {
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var token = GenerateEncryptedToken(GetSigningCredentials(), principal.Claims);
        return token;
    }
    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddDays(2),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    public async Task UpdateLiveStatus(int userId, bool isLive, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsLive != isLive);
        if (user is not null)
        {
            user.IsLive = isLive;
            var result= await _userManager.UpdateAsync(user);
        }
    }
    public async Task<Common.Models.API.Entities.User> GetApplicationUserDto(int userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserDto:{userId}";
        var result = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.Where(x => x.Id == userId).Include(x => x.UserUserRole).ThenInclude(x => x.UserRole).ProjectToType<Common.Models.API.Entities.User>().FirstAsync(cancellation), Options);
        return result;
    }
    public async Task<List<Common.Models.API.Entities.User>?> GetUsers(string? tenantId, CancellationToken cancellation = default)
    {
        var key = $"GetUserDtoListWithTenantId:{tenantId}";
        Func<string?, CancellationToken, Task<List<Common.Models.API.Entities.User>?>> getUsersByTenantId = async (tenantId, token) =>
        {
                return await _userManager.Users.Include(x => x.UserUserRole).ThenInclude(x => x.UserRole)
                    .ProjectToType<Common.Models.API.Entities.User>().ToListAsync();
        };
        var result = await _cache.GetOrAddAsync(key, () => getUsersByTenantId(tenantId,cancellation), Options);
        return result;
    }
}
