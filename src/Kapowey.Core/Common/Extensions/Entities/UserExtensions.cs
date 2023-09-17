using Kapowey.Core.Common.Models;
using Kapowey.Core.Entities;

namespace Kapowey.Core.Common.Extensions.Entities
{
    public static class UserExtensions
    {
        public static bool IsInRole(this User user, string role)
        {
            return user?.Roles?.Any(x => string.Equals(role, x, System.StringComparison.OrdinalIgnoreCase)) ?? false;
        }

        public static bool IsAdmin(this User user) => user?.IsInRole(UserRoleRegistry.AdminRoleName) ?? false;

        public static bool IsManager(this User user) => user.IsAdmin() || (user?.IsInRole(UserRoleRegistry.ManagerRoleName) ?? false);

        public static bool IsEditor(this User user) => user.IsManager() || (user?.IsInRole(UserRoleRegistry.EditorRoleName) ?? false);

        public static bool IsContributor(this User user) => user.IsEditor() || (user?.IsInRole(UserRoleRegistry.ContributorRoleName) ?? false);
    }
}
