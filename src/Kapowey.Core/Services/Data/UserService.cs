using System.Security.Claims;
using System.Text.RegularExpressions;
using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Extensions;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;
using Kapowey.Core.Enums;
using Kapowey.Core.Imaging;
using Kapowey.Core.Persistance;
using LazyCache;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NodaTime;
using API = Kapowey.Core.Common.Models.API.Entities;
using Image = SixLabors.ImageSharp.Image;

namespace Kapowey.Core.Services.Data
{
    /// <summary>
    /// This service does both EF DAL entities (used by UserManager) and API  This is not standard as most Services are APIServices accepting and serving only API entitiy models.
    /// </summary>
    public sealed class UserService : ServiceBase, IUserService
    {
        private const string _userRegionKey = "urn:region:users";
        private const string _userByUserNameKey = "urn:user:username:{0}";
        private const string _userByUserEmailKey = "urn:user:email:{0}";
        private const string _userByUserIdKey = "urn:user:by:id:{0}";
        private const string _userByUserApiKey = "urn:user:by:apiKey:{0}";

        private IJwtService JwtService { get; }

        private IPasswordHasher<User> PasswordHasher { get; }

        private ILogger<UserService> Logger { get; }

        private IImageService ImageService { get; }

        private IKapoweyHttpContext HttpContext { get; }
        public IMailService MailService { get; }
        
        public IStringLocalizer<UserService> StringLocalizer { get; }
        public UserManager<User> UserManager { get; }

        public UserService(
            AppConfigurationSettings appSettings,
            ILogger<UserService> logger,
            IAppCache cacheManager,
            KapoweyContext dbContext,
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher,
            IImageService imageService,
            IKapoweyHttpContext httpContext,
            IMailService mailService,
            IStringLocalizer<UserService> stringLocalizer,
            UserManager<User> userManager)
             : base(appSettings, cacheManager, dbContext)
        {
            Logger = logger;
            JwtService = jwtService;
            PasswordHasher = passwordHasher;
            ImageService = imageService;
            HttpContext = httpContext;
            MailService = mailService;
            StringLocalizer = stringLocalizer;
            UserManager = userManager;
        }

        private Task<int?> GetUserIdForUserName(string userName) => CacheManager.GetOrAddAsync(_userByUserNameKey.ToCacheKey(userName), () => GetUserIdForUserNameAction(userName), Options);

        public Task<int?> GetUserIdByEmail(string email) => CacheManager.GetOrAddAsync(_userByUserEmailKey.ToCacheKey(email), () => GetUserIdForEmailAction(email), Options);

        private Task<int?> GetUserIdForUserApiKey(Guid? apiKey) => apiKey == null ? null : CacheManager.GetOrAddAsync(_userByUserApiKey.ToCacheKey(apiKey), () => GetUserIdForUserApiKeyAction(apiKey.Value), Options);

        public Task<User> GetUserById(int userId) => CacheManager.GetOrAddAsync(_userByUserIdKey.ToCacheKey(userId), () => GetUserByUserIdAction(userId), Options);

        private async Task<User> GetUserByApiKey(Guid? apiKey) => await GetUserById(await GetUserIdForUserApiKey(apiKey).ConfigureAwait(false) ?? 0).ConfigureAwait(false);

        private Task<int?> GetUserIdForUserNameAction(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            return DbContext.User
                            .Where(x => x.NormalizedUserName == userName.ToUpper())
                            .Select(x => (int?)x.Id)
                            .FirstOrDefaultAsync();
        }

        private Task<int?> GetUserIdForEmailAction(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                return null;
            }
            return DbContext.User
                .Where(x => x.NormalizedEmail == email.ToUpper())
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();
        }

        private Task<int?> GetUserIdForUserApiKeyAction(Guid apiKey)
        {
            return DbContext.User
                            .Where(x => x.ApiKey == apiKey)
                            .Select(x => (int?)x.Id)
                            .FirstOrDefaultAsync();
        }

        private Task<User> GetUserByUserIdAction(int userId)
        {
            return DbContext.User
                            .Include(x => x.Claims)
                            .Include(x => x.UserUserRole).ThenInclude(x => x.UserRole).ThenInclude(x => x.Claims)
                            .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<IServiceResponse<AuthenticateResponse>> AuthenticateAsync(API.ApiApplication application, AuthenticateRequest request)
        {
            var userId = await GetUserIdByEmail(request.Email).ConfigureAwait(false) ?? 0;
            var user = await GetUserById(userId).ConfigureAwait(false);

            if (user == null)
            {
                return new ServiceResponse<AuthenticateResponse>(new ServiceResponseMessage("Invalid User", ServiceResponseMessageType.Authentication));
            }
            if(user.EmailConfirmed != true)
            {
                return new ServiceResponse<AuthenticateResponse>(new ServiceResponseMessage("User must validate email", ServiceResponseMessageType.Authentication));
            }
            switch (PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password))
            {
                case PasswordVerificationResult.Success:
                    var apiUser = user.Adapt<API.UserInfo>();
                    apiUser.Roles = user.UserRoles?.Select(x => x.UserRole.Name);
                    var claims = new List<Claim>();
                    claims.AddRange(user.Claims?.Select(x => new Claim(x.ClaimType, x.ClaimValue)) ?? Enumerable.Empty<Claim>());
                    claims.AddRange(user.UserRoles?.Select(x => x.UserRole)?
                                                   .SelectMany(x => x.Claims)?
                                                   .Where(x => x != null)
                                                   .Select(x => new Claim(x.ClaimType, x.ClaimValue)) ?? Enumerable.Empty<Claim>());
                    claims.AddRange(user.UserRoles?.Select(x => new Claim(ClaimTypes.Role, x.UserRole.Name)) ?? Enumerable.Empty<Claim>());
                    apiUser.Claims = claims;
                    var model = new AuthenticateResponse(apiUser, JwtService.GenerateSecurityToken(apiUser));
                    user.LastAuthenticateDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
                    user.SuccessfulAuthenticateCount = user.SuccessfulAuthenticateCount.HasValue ? user.SuccessfulAuthenticateCount + 1 : 1;
                    await DbContext.SaveChangesAsync().ConfigureAwait(false);
                    CacheManager.Remove(_userByUserIdKey.ToCacheKey(userId));
                    return new ServiceResponse<AuthenticateResponse>(model, new ServiceResponseMessage(ServiceResponseMessageType.Ok));

                case PasswordVerificationResult.SuccessRehashNeeded:
                    throw new NotImplementedException();
            }
            return new ServiceResponse<AuthenticateResponse>(new ServiceResponseMessage("Invalid Authorization Attempt", ServiceResponseMessageType.Authentication));
        }

        public Task<IFileOperationResponse<IImage>> GetUserAvatarImageAsync(Guid id, int width, int height, EntityTagHeaderValue etag = null)
        {
            return ImageService.GetImageAsyncAction(ImageType.UserAvatar, API.User.CacheRegionUrn(id), id, width, height, async () =>
            {
                var userData = await GetUserByApiKey(id).ConfigureAwait(false);
                if (userData == null)
                {
                    return null;
                }
                var user = userData.Adapt<User>();
                IImage image = new Common.Interfaces.Image()
                {
                    CreatedDate = user.CreatedDate.Value
                };
                var userImageFilename = ImageService.ImagePath(ImageType.UserAvatar, user.ApiKey.Value);
                try
                {
                    if (File.Exists(userImageFilename))
                    {
                        image.Bytes = await File.ReadAllBytesAsync(userImageFilename).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error Reading Image File [{userImageFilename}]");
                }
                return image;
            }, etag);
        }

        public async Task<IServiceResponse<API.User>> ByIdAsync(User user, Guid apiKey)
        {
            var userData = await GetUserByApiKey(apiKey).ConfigureAwait(false);
            if (userData == null)
            {
                return new ServiceResponse<API.User>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            return new ServiceResponse<API.User>(userData.Adapt<API.User>());
        }

        public static PasswordScore CheckPasswordStrength(string password)
        {
            int score = 0;

            if ((password?.Length ?? 0) < 1)
            {
                return PasswordScore.Blank;
            }
            if (password.Length < 4)
            {
                return PasswordScore.VeryWeak;
            }
            if (password.Length >= 8)
            {
                score++;
            }
            if (password.Length >= 12)
            {
                score++;
            }
            if (Regex.Match(password, @"\d+", RegexOptions.IgnoreCase).Success)
            {
                score++;
            }
            if (Regex.Match(password, "[a-z]", RegexOptions.IgnoreCase).Success &&
              Regex.Match(password, "[A-Z]", RegexOptions.IgnoreCase).Success)
            {
                score++;
            }
            if (Regex.Match(password, ".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.IgnoreCase).Success)
            {
                score++;
            }
            return (PasswordScore)score;
        }

        public async Task<IServiceResponse<Guid>> AddAsync(User user, API.User add)
        {
            var data = add.Adapt<User>();
            data.ApiKey = Guid.NewGuid();
            data.CreatedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            await DbContext.Users.AddAsync(data).ConfigureAwait(false);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogWarning($"User `{ user }` add: User `{ data }`.");
            return new ServiceResponse<Guid>(data.ApiKey.Value);
        }

        public async Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKey)
        {
            var userToDelete = await GetUserByApiKey(apiKey).ConfigureAwait(false);
            if (userToDelete == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ apiKey }]", ServiceResponseMessageType.NotFound));
            }
            DbContext.User.Remove(userToDelete);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
            Logger.LogWarning($"User `{ user }` deleted: User `{ userToDelete }`.");
            return new ServiceResponse<bool>(true);
        }

        public static bool IsNewPasswordStrongEnough(string password)
        {
            return (CheckPasswordStrength(password)) switch
            {
                PasswordScore.Medium or PasswordScore.Strong or PasswordScore.VeryStrong => true,
                _ => false,
            };
        }

        public async Task<IPagedResponse<API.UserInfo>> ListAsync(User user, PagedRequest request)
        {
            if (!request.IsValid)
            {
                return new PagedResponse<API.UserInfo>(new ServiceResponseMessage("Invalid Request", ServiceResponseMessageType.Error));
            }
            return await CreatePagedResponse<User, API.UserInfo>(DbContext.User, request).ConfigureAwait(false);
        }

        public async Task<IServiceResponse<bool>> ModifyAsync(User user, API.User modify)
        {
            var userId = await GetUserIdForUserApiKey(user.ApiKey).ConfigureAwait(false);
            var data = await GetUserByUserIdAction(userId.Value).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid ApiKey [{ modify.ApiKey }]", ServiceResponseMessageType.NotFound));
            }
            if (data.ConcurrencyStamp != modify.ConcurrencyStamp)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage("Invalid ModifyToken", ServiceResponseMessageType.Validation));
            }
            data.Status = Status.Edited;
            data.UserName = modify.UserName;
            data.NormalizedUserName = modify.UserName.ToUpper();
            if (!String.Equals(data.Email, modify.Email))
            {
                data.Email = modify.Email;
                data.NormalizedEmail = modify.Email.ToUpper();
                data.EmailConfirmed = false;
            }
            if (!String.Equals(data.PhoneNumber, modify.PhoneNumber))
            {
                data.PhoneNumber = modify.PhoneNumber;
                data.PhoneNumberConfirmed = false;
            }
            data.ConcurrencyStamp = Guid.NewGuid().ToString();
            data.IsPublic = modify.IsPublic;
            data.LockoutEnabled = modify.LockoutEnabled;
            data.LockoutEnd = modify.LockoutEnd;
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = user.Id;
            data.ProfileAbout = modify.ProfileAbout;
            data.Tags = modify.Tags;
            data.TwoFactorEnabled = modify.TwoFactorEnabled;

            if (modify.AvatarUrl != null)
            {
                var imageBytes = await ImageHelper.ConvertToPngFormatViaSixLabors(ImageHelper.ImageDataFromUrl(modify.AvatarUrl));
                if(imageBytes != null)
                {
                    await File.WriteAllBytesAsync(ImageService.ImagePath(ImageType.UserAvatar, data.ApiKey.Value), imageBytes);
                }
            }
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0);
        }

        public async Task<IServiceResponse<bool>> ValidateEmail(API.ApiApplication application, string email, string securityToken)
        {
            var userId = await GetUserIdByEmail(email).ConfigureAwait(false);
            var data = await GetUserByUserIdAction(userId.Value).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid Email [{ email }]", ServiceResponseMessageType.NotFound));
            }
            if (data.SecurityStamp != securityToken)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage("Invalid SecurityToken", ServiceResponseMessageType.Validation));
            }
            data.ConcurrencyStamp = Guid.NewGuid().ToString();
            data.SecurityStamp = Guid.NewGuid().ToString();
            data.EmailConfirmed = true;
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = data.Id;
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0);
        }

        public async Task<IServiceResponse<bool>> ResetPassword(API.ApiApplication application, string email, string password, string hmacToken, string resetUrl)
        {
            var userId = await GetUserIdByEmail(email).ConfigureAwait(false);
            var data = await GetUserByUserIdAction(userId.Value).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid Email [{ email }]", ServiceResponseMessageType.NotFound));
            }
            if (!IsNewPasswordStrongEnough(password))
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage("Password is not strong enough, try making it longer, adding numbers or special characters.", ServiceResponseMessageType.Validation));
            }
            if(!application.IsValidToken(hmacToken, resetUrl))
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage("Reset security token is invalid. Likely the token has expired, send a new reset email and try again.", ServiceResponseMessageType.Validation));
            }
            data.PasswordHash = PasswordHasher.HashPassword(data, password);
            data.ConcurrencyStamp = Guid.NewGuid().ToString();
            data.ModifiedDate = Instant.FromDateTimeUtc(DateTime.UtcNow);
            data.ModifiedUserId = data.Id;
            var modified = await DbContext.SaveChangesAsync().ConfigureAwait(false);
            return new ServiceResponse<bool>(modified > 0, new ServiceResponseMessage(ServiceResponseMessageType.Ok));
        }


        public async Task<IServiceResponse<bool>> SendPasswordResetEmail(API.ApiApplication application, string email, string returnUrl)
        {
            var result = false;
            var userId = await GetUserIdByEmail(email).ConfigureAwait(false);
            var data = await GetUserByUserIdAction(userId.Value).ConfigureAwait(false);
            if (data == null)
            {
                return new ServiceResponse<bool>(new ServiceResponseMessage($"Invalid Email [{ email }]", ServiceResponseMessageType.NotFound));
            }
            var token = await UserManager.GeneratePasswordResetTokenAsync(data);
            var subject = StringLocalizer["Verify your recovery email"];
            //var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates" ,"_recoverypassword.txt");
            var sendResult = MailService.SendAsync(data.Email, subject, "_recoverypassword", new { AppName = "Kapowey", Email = data.Email, Token = token });            
            return new ServiceResponse<bool>(result, new ServiceResponseMessage(sendResult.IsCompletedSuccessfully ? ServiceResponseMessageType.Ok : ServiceResponseMessageType.Error));
        }

        public async Task<IServiceResponse<int>> Register(API.ApiApplication application, UserRegistration model)
        {
            try
            {
                var newUser = new User
                {
                    UserName = model.UserName,
                    NormalizedUserName = model.UserName.ToUpper(),
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper()
                };
                if(!IsNewPasswordStrongEnough(model.Password))
                {
                    return new ServiceResponse<int>(new ServiceResponseMessage("Password is not strong enough, try making it longer, adding numbers or special characters.", ServiceResponseMessageType.Validation));
                }
                newUser.PasswordHash = PasswordHasher.HashPassword(newUser, model.Password);
                await DbContext.User.AddAsync(newUser).ConfigureAwait(false);
                await DbContext.SaveChangesAsync().ConfigureAwait(false);
                var isFirstUser = (await DbContext.Users.CountAsync().ConfigureAwait(false) == 1);
                if (isFirstUser) // First user to register
                {
                    var adminRole = await DbContext.UserRole.FirstOrDefaultAsync(x => x.Name == UserRoleRegistry.AdminRoleName).ConfigureAwait(false);
                    if (adminRole == null)
                    {
                        // Create User Roles
                        await DbContext.UserRole.AddAsync(new UserRole
                        {
                            Name = UserRoleRegistry.AdminRoleName,
                            NormalizedName = UserRoleRegistry.AdminRoleName.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        }).ConfigureAwait(false);
                        await DbContext.UserRole.AddAsync(new UserRole
                        {
                            Name = UserRoleRegistry.ManagerRoleName,
                            NormalizedName = UserRoleRegistry.ManagerRoleName.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        }).ConfigureAwait(false);
                        await DbContext.UserRole.AddAsync(new UserRole
                        {
                            Name = UserRoleRegistry.EditorRoleName,
                            NormalizedName = UserRoleRegistry.EditorRoleName.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        }).ConfigureAwait(false);
                        await DbContext.UserRole.AddAsync(new UserRole
                        {
                            Name = UserRoleRegistry.ContributorRoleName,
                            NormalizedName = UserRoleRegistry.ContributorRoleName.ToUpper(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        }).ConfigureAwait(false);
                        await DbContext.SaveChangesAsync().ConfigureAwait(false);
                        adminRole = await DbContext.UserRole.FirstOrDefaultAsync(x => x.Name == UserRoleRegistry.AdminRoleName).ConfigureAwait(false);
                    }
                    if (adminRole == null)
                    {
                        throw new Exception($"Unable to add initial user to { UserRoleRegistry.AdminRoleName } role");
                    }
                    // Add user as Admin
                    await DbContext.UserUserRole.AddAsync(new UserUserRole
                    {
                        UserId = newUser.Id,
                        RoleId = adminRole.Id
                    }).ConfigureAwait(false);
                    await DbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                var subject =StringLocalizer["Welcome to Kapowey!"];
                var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates" ,"_welcome.txt");
                var sendResult = MailService.SendAsync(newUser.Email, subject, "_welcome", new { AppName = "Kapowey", Email = newUser.Email, UserName = newUser.UserName });
                return new ServiceResponse<int>(newUser.Id, new ServiceResponseMessage(sendResult.IsCompletedSuccessfully ? ServiceResponseMessageType.Ok : ServiceResponseMessageType.Error));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(Register), model);
            }
            return new ServiceResponse<int>(new ServiceResponseMessage("An Error has occured", ServiceResponseMessageType.Error));
        }
    }
}