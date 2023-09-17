using FluentValidation;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class Grade : GradeInfo
    {
        public string Notes { get; set; }
    }

    public sealed class GradeValidator : AbstractValidator<Grade>
    {
        public GradeValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Grade name");

            RuleFor(p => p.Abbreviation)
                .NotEmpty()
                .MaximumLength(6)
                .WithMessage("Please provide a valid Grade Abbreviation");
        }
    }
}