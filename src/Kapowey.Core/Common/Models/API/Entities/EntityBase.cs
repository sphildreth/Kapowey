using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kapowey.Core.Enums;
using Mapster;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public abstract class EntityBase
    {
        [Required]
        [JsonPropertyName("id")]
        public Guid? ApiKey { get; set; }

        [Required]
        [JsonPropertyName("modifyToken")]
        public string ConcurrencyStamp { get; set; }

        [JsonIgnore]
        public virtual Instant? CreatedDate { get; set; }

        [AdaptIgnore]
        public UserInfo CreatedUser { get; set; }

        [JsonIgnore]
        public virtual int CreatedUserId { get; set; }

        public virtual string Description { get; set; }

        [JsonIgnore]
        public virtual Instant? ModifiedDate { get; set; }

        [AdaptIgnore]
        public virtual UserInfo ModifiedUser { get; set; }

        [JsonIgnore]
        public virtual int? ModifiedUserId { get; set; }

        [JsonIgnore]
        public virtual Instant? ReviewedDate { get; set; }

        [AdaptIgnore]
        public virtual UserInfo ReviewedUser { get; set; }

        [JsonIgnore]
        public virtual int? ReviewedUserId { get; set; }

        public virtual Status Status { get; set; }

        public virtual string[] Tags { get; set; }

        [StringLength(1000)]
        [DataType(DataType.Url)]
        public virtual string Url { get; set; }
    }
}