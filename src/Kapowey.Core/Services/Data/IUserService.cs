using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models;
using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Common.Models.API.Entities;
using Microsoft.Net.Http.Headers;

namespace Kapowey.Core.Services.Data
{
    public interface IUserService : IApiEntityService<User>, IApiEntityListService<UserInfo>
    {                
        Task<IServiceResponse<int>> Register(ApiApplication application, UserRegistration user);

        Task<IServiceResponse<bool>> ValidateEmail(ApiApplication application, string email, string securityToken);

        Task<IServiceResponse<bool>> SendPasswordResetEmail(ApiApplication application, string email, string returnUrl);

        Task<IServiceResponse<bool>> ResetPassword(ApiApplication application, string email, string password, string hmacToken, string resetUrl);

        Task<IServiceResponse<AuthenticateResponse>> AuthenticateAsync(ApiApplication application, AuthenticateRequest request);

        Task<IFileOperationResponse<IImage>> GetUserAvatarImageAsync(Guid id, int width, int height, EntityTagHeaderValue etag = null);
    }
}