using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class IssueTypeInfo : EntityBase
    {
        [Required]
        [StringLength(2)]
        public string Abbreviation { get; set; }

        [JsonIgnore]
        public int IssueTypeId { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }
    }

    public sealed class IssueTypeInfoValidator : AbstractValidator<IssueTypeInfo>
    {
        public IssueTypeInfoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Issue Type name");

            RuleFor(p => p.Abbreviation)
                .NotEmpty()
                .MaximumLength(2)
                .WithMessage("Please provider a valid Issue Type Abbreviation");
        }
    }
}