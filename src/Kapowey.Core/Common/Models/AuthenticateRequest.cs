using System.ComponentModel.DataAnnotations;

namespace Kapowey.Core.Common.Models
{
    [Serializable]
    public sealed class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}