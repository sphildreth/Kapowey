using System.Text.Json.Serialization;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    /// <summary>
    /// User detail record
    /// </summary>
    public sealed class User : UserInfo
    {
        public override Instant? CreatedDate { get; set; }

        public bool? IsPublic { get; set; }

        public string ProfileAbout { get; set; }

        public bool? LockoutEnabled { get; set; }

        public Instant? LockoutEnd { get; set; }

        public override Instant? ModifiedDate { get; set; }

        public string PhoneNumber { get; set; }

        public override Status Status { get; set; }

        public bool? TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public string NormalizedEmail { get; set; }

        [JsonIgnore]
        public string NormalizedUserName { get; set; }
    }
}