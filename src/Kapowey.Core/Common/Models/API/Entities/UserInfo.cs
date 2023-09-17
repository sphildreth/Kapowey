using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using FluentValidation;
using Mapster;
using NodaTime;

namespace Kapowey.Core.Common.Models.API.Entities
{
    [Serializable]
    public class UserInfo : EntityBase
    {
        [AdaptIgnore]
        [JsonIgnore]
        public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();

        [JsonIgnore]
        public override Instant? CreatedDate { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }

        [AdaptIgnore]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [AdaptIgnore]
        [JsonIgnore]
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

        [JsonIgnore]
        public int UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        public string AvatarUrl { get; set; }

        public static string CacheRegionUrn(Guid Id) => string.Format("urn:user:{0}", Id);
    }

    public class UserInfoValidator : AbstractValidator<UserInfo>
    {
        public UserInfoValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty()
                .MaximumLength(255)
                .WithMessage("Please provide a valid user name");

            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255)
                .WithMessage("Please provide a valid Email address");

            //RuleFor(x => x.Password)
            //    .Must(password => UserService.IsNewPasswordStrongEnough(password))
            //    .WithMessage("Password is not strong enough, try making it longer, adding numbers or special characters.");
        }
    }
}