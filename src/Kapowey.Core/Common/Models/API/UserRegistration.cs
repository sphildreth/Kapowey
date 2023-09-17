using System.ComponentModel.DataAnnotations;

namespace Kapowey.Core.Common.Models.API
{
    [Serializable]
    public sealed class UserRegistration
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        public string ValiationReturnUrl { get; set; }
    }
}
