using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class PublisherCategory : EntityBase
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        public PublisherCategory ParentPublisherCategory { get; set; }

        [JsonIgnore]
        public int ParentPublisherCategoryId { get; set; }

        [JsonIgnore]
        public int PublisherCategoryId { get; set; }

        [Required]
        [StringLength(10)]
        public string ShortName { get; set; }
    }

    public sealed class PublisherCategoryInfoValidator : AbstractValidator<PublisherCategory>
    {
        public PublisherCategoryInfoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Publisher Category name");

            RuleFor(p => p.ShortName)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Please provide a valid Publisher Category short name");
        }
    }
}