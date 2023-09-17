using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Newtonsoft.Json;

namespace Kapowey.Core.Common.Models.API.Entities
{
    public class GradeTerm : EntityBase
    {
        [Required]
        [StringLength(500)]
        public virtual string Name { get; set; }

        [JsonIgnore]
        public int GradeTermId { get; set; }

        public int SortOrder { get; set; }
    }

    public sealed class GradeTermValidator : AbstractValidator<GradeTerm>
    {
        public GradeTermValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Grade Term name"); 
        }
    }
}