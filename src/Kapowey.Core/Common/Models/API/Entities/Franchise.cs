using FluentValidation;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class Franchise : FranchiseInfo
    {
        public int? GcdId { get; set; }

        public override Instant? ModifiedDate { get; set; }

        public override Instant? ReviewedDate { get; set; }

        public int? YearBegan { get; set; }

        public int? YearEnd { get; set; }
    }

    public sealed class FranchiseValidator : AbstractValidator<Franchise>
    {
        public FranchiseValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Franchise name");

            RuleFor(p => p.ShortName)
                .NotEmpty()
                .MaximumLength(20)
                .WithMessage("Please provide a valid Franchise short name");
        }
    }
}