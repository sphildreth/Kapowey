using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class GradeInfo : EntityBase
    {
        [JsonIgnore]
        public virtual int GradeId { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(6)]
        public virtual string Abbreviation { get; set; }

        public decimal Scale { get; set; }

        public int SortOrder { get; set; }

        public bool IsBasicGrade { get; set; }
    }
}