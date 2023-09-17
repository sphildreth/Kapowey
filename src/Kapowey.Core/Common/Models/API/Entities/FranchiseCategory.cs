using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class FranchiseCategory : EntityBase
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [AdaptIgnore]
        public FranchiseCategory ParentFranchiseCategory { get; set; }

        [JsonIgnore]
        public int ParentFranchiseCategoryId { get; set; }

        [JsonIgnore]
        public int SeriesCategoryId { get; set; }

        [Required]
        [StringLength(10)]
        public string ShortName { get; set; }

        [JsonIgnore]
        public virtual int FranchiseCategoryId { get; set; }
    }

    public sealed class FranchiseCategoryValidator : AbstractValidator<FranchiseCategory>
    {
        public FranchiseCategoryValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Franchise Category name");

            RuleFor(p => p.ShortName)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Please provide a valid Franchise Category short name");
        }
    }
}