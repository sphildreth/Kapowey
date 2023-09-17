using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mapster;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class Collection : EntityBase
    {
        [AdaptIgnore]
        public UserInfo User { get; set; }

        public bool IsPublic { get; set; }

        public Instant? LastActivityDate { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public int CollectionId { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string ShortName { get; set; }

        public int SortOrder { get; set; }
    }
}