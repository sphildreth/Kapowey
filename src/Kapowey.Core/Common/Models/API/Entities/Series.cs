using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Kapowey.Core.Enums;
using Mapster;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class Series : SeriesInfo
    {
        [Required]
        [StringLength(3)]
        public string CultureCode { get; set; }

        [AdaptIgnore]
        public IssueInfo FirstIssue { get; set; }

        public Guid? FirstIssueId { get; set; }

        public int? GcdId { get; set; }

        [AdaptIgnore]
        public IssueInfo LastIssue { get; set; }

        public Guid? LastIssueId { get; set; }

        [AdaptIgnore]
        public Rating Rating { get; set; }

        public int? YearBegan { get; set; }

        public int? YearEnd { get; set; }
    }

    public sealed class SeriesValidator : AbstractValidator<Series>
    {
        public SeriesValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Series name");

            RuleFor(p => p.ShortName)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Please provide a valid Series short name");

            RuleFor(p => p.CultureCode)
                .NotEmpty()
                .MaximumLength(3)
                .WithMessage("Please provide a valid Series culture code");
        }
    }
}