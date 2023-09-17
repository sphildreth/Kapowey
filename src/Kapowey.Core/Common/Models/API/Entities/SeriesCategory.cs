using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class SeriesCategory : EntityBase
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [AdaptIgnore]
        public SeriesCategory ParentSeriesCategory { get; set; }

        [JsonIgnore]
        public int ParentSeriesCategoryId { get; set; }

        [JsonIgnore]
        public int SeriesCategoryId { get; set; }

        [Required]
        [StringLength(10)]
        public string ShortName { get; set; }
    }

    public sealed class SeriesCategoryValidator : AbstractValidator<SeriesCategory>
    {
        public SeriesCategoryValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Series Category name");

            RuleFor(p => p.ShortName)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Please provide a valid Series Category short name");
        }
    }
}