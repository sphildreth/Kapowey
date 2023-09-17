using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class FranchiseInfo : EntityBase
    {
        [AdaptIgnore]
        public virtual FranchiseInfo ParentFranchise { get; set; }

        [JsonIgnore]
        public virtual int? ParentFranchiseId { get; set; }

        [AdaptIgnore]
        public PublisherInfo Publisher { get; set; }

        [JsonIgnore]
        public int? FranchiseCategoryId { get; set; }

        [AdaptIgnore]
        public FranchiseCategory FranchiseCategory { get; set; }

        [JsonIgnore]
        public int? PublisherId { get; set; }

        [StringLength(500)]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public virtual int FranchiseId { get; set; }

        [Required]
        [StringLength(20)]
        public virtual string ShortName { get; set; }
    }
}