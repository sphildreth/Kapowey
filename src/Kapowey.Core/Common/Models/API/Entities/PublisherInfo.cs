using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    /// <summary>
    /// Minimum Publisher record used by most API operations
    /// </summary>
    [Serializable]
    public class PublisherInfo : EntityBase
    {
        [AdaptIgnore]
        public virtual PublisherCategory Category { get; set; }

        public int FranchiseCount { get; set; }

        public int IssueCount { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string Name { get; set; }

        [AdaptIgnore]
        public virtual PublisherInfo ParentPublisher { get; set; }

        [JsonIgnore]
        public virtual int ParentPublisherId { get; set; }

        [JsonIgnore]
        public virtual int PublisherCategoryId { get; set; }

        [JsonIgnore]
        public virtual int PublisherId { get; set; }

        public int SeriesCount { get; set; }

        [Required]
        [StringLength(10)]
        public virtual string ShortName { get; set; }

        public string ImageUrl { get; set; }

        public static string CacheRegionUrn(Guid Id) => string.Format("urn:publisher:{0}", Id);
    }
}