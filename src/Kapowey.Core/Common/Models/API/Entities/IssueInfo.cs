using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mapster;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class IssueInfo : EntityBase
    {
        public Instant? KeyDate { get; set; }

        [JsonIgnore]
        public virtual int IssueId { get; set; }

        [AdaptIgnore]
        public SeriesInfo Series { get; set; }

        [JsonIgnore]
        public int SeriesId { get; set; }

        [StringLength(10)]
        public string Number { get; set; }

        [StringLength(25)]
        public string Barcode { get; set; }

        [AdaptIgnore]
        public IssueTypeInfo IssueType { get; set; }

        public int IssueTypeId { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(20)]
        public string ShortTitle { get; set; }

        public int SortOrder { get; set; }
    }
}