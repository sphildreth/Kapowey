using Kapowey.Core.Enums;
using Mapster;
using Newtonsoft.Json;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class CollectionIssueInfo : EntityBase
    {
        public int SortOrder { get; set; }

        public Rating Rating { get; set; }

        public GradeInfo Grade { get; set; }

        public decimal? PricePaid { get; set; }

        [JsonIgnore]
        public int GradeId { get; set; }

        [AdaptIgnore]
        public IssueInfo Issue { get; set; }

        [JsonIgnore]
        public virtual int IssueId { get; set; }

        [AdaptIgnore]
        public Collection Collection { get; set; }

        [JsonIgnore]
        public virtual int CollectionId { get; set; }

        [JsonIgnore]
        public virtual int CollectionIssueId { get; set; }
    }
}