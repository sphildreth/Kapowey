using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class SeriesInfo : EntityBase
    {
        public FranchiseInfo Franchise { get; set; }

        [JsonIgnore]
        public int FranchiseId { get; set; }

        [JsonIgnore]
        public int SeriesCategoryId { get; set; }

        public SeriesCategory SeriesCategory { get; set; }

        [AdaptIgnore]
        public GenreInfo Genre { get; set; }

        [JsonIgnore]
        public int? GenreId { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public virtual int SeriesId { get; set; }

        [Required]
        [StringLength(20)]
        public virtual string ShortName { get; set; }
    }
}