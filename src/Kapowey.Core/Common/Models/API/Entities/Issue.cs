using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Kapowey.Core.Enums;
using Mapster;
using Newtonsoft.Json;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class Issue : IssueInfo
    {
        [Required]
        [StringLength(3)]
        public string CultureCode { get; set; }

        public int? GcdId { get; set; }

        [StringLength(25)]
        public string ISBN { get; set; }

        [AdaptIgnore]
        public Rating Rating { get; set; }

        [AdaptIgnore]
        public IssueInfo ReprintOfIssue { get; set; }

        [JsonIgnore]
        public int ReprintOfIssueId { get; set; }

        [StringLength(500)]
        public string VariantTitle { get; set; }

        public decimal? CoverPrice { get; set; }
    }

    public sealed class IssueValidator : AbstractValidator<Issue>
    {
        public IssueValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Issue title");

            RuleFor(p => p.KeyDate)
                .NotEmpty()
                .WithMessage("Please provide a valid Issue key date");

            RuleFor(p => p.CultureCode)
                .NotEmpty()
                .MaximumLength(3)
                .WithMessage("Please provide a valid Issue culture code");
        }
    }
}