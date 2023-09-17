using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IJwtService
    {
        string GenerateSecurityToken(UserInfo user);
    }
}