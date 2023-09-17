using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Kapowey.Core.Enums;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public sealed class Publisher : PublisherInfo
    {
        [Required]
        [StringLength(3)]
        public string CountryCode { get; set; }

        public int? GcdId { get; set; }

        public int? YearBegan { get; set; }

        public int? YearEnd { get; set; }

        public override string ToString()
        {
            return $"Id [{ PublisherId }] Name [{ Name }]";
        }

        public Publisher()
        {
            Status = Status.New;
        }
    }

    public sealed class PublisherValidator : AbstractValidator<Publisher>
    {
        public PublisherValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Please provider a valid Publisher name");

            RuleFor(p => p.ShortName)
                .NotEmpty()
                .MaximumLength(10)
                .WithMessage("Please provide a valid Publisher short name");

            RuleFor(p => p.CountryCode)
                .NotEmpty()
                .MaximumLength(3)
                .WithMessage("Please provide a valid Publisher country code");
        }
    }
}