using System.ComponentModel.DataAnnotations;

namespace Kapowey.Core.Common.Models.API
{
    [Serializable]
    public sealed class UserModifyAvatar
    {
        [Required]
        public string AvatarUrl { get; set; }

        [Required]
        public Guid ModifyToken { get; set; }
    }
}
