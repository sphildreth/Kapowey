using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class GenreInfo : EntityBase
    {
        [JsonIgnore]
        public int GenreId { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string Name { get; set; }

        [AdaptIgnore]
        public virtual GenreInfo ParentGenre { get; set; }

        [JsonIgnore]
        public virtual int ParentGenreId { get; set; }

        [Required]
        [StringLength(10)]
        public virtual string ShortName { get; set; }
    }
}