using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("user")]
    public partial class User : IdentityUser<int>, IEntity<int>
    {
        public User()
        {
            ApiApplicationCreatedUser = new HashSet<ApiApplication>();
            ApiApplicationModifiedUser = new HashSet<ApiApplication>();
            ApiApplicationReviewedUser = new HashSet<ApiApplication>();
            FranchiseCategoryCreatedUser = new HashSet<FranchiseCategory>();
            FranchiseCategoryModifiedUser = new HashSet<FranchiseCategory>();
            FranchiseCategoryReviewedUser = new HashSet<FranchiseCategory>();
            FranchiseCreatedUser = new HashSet<Franchise>();
            FranchiseModifiedUser = new HashSet<Franchise>();
            FranchiseReviewedUser = new HashSet<Franchise>();
            GenreCreatedUser = new HashSet<Genre>();
            GenreModifiedUser = new HashSet<Genre>();
            GenreReviewedUser = new HashSet<Genre>();
            GradeCreatedUser = new HashSet<Grade>();
            GradeModifiedUser = new HashSet<Grade>();
            GradeReviewedUser = new HashSet<Grade>();
            GradeTermCreatedUser = new HashSet<GradeTerm>();
            GradeTermModifiedUser = new HashSet<GradeTerm>();
            GradeTermReviewedUser = new HashSet<GradeTerm>();
            InverseModifiedUser = new HashSet<User>();
            IssueCreatedUser = new HashSet<Issue>();
            IssueModifiedUser = new HashSet<Issue>();
            IssueReviewedUser = new HashSet<Issue>();
            IssueTypeCreatedUser = new HashSet<IssueType>();
            IssueTypeModifiedUser = new HashSet<IssueType>();
            IssueTypeReviewedUser = new HashSet<IssueType>();
            PublisherCategoryCreatedUser = new HashSet<PublisherCategory>();
            PublisherCategoryModifiedUser = new HashSet<PublisherCategory>();
            PublisherCategoryReviewedUser = new HashSet<PublisherCategory>();
            PublisherCreatedUser = new HashSet<Publisher>();
            PublisherModifiedUser = new HashSet<Publisher>();
            PublisherReviewedUser = new HashSet<Publisher>();
            SeriesCategoryCreatedUser = new HashSet<SeriesCategory>();
            SeriesCategoryModifiedUser = new HashSet<SeriesCategory>();
            SeriesCategoryReviewedUser = new HashSet<SeriesCategory>();
            SeriesCreatedUser = new HashSet<Series>();
            SeriesModifiedUser = new HashSet<Series>();
            SeriesReviewedUser = new HashSet<Series>();

            UserLogin = new HashSet<UserLogin>();
            UserToken = new HashSet<UserToken>();
            UserUserRole = new HashSet<UserUserRole>();

            SecurityStamp = Guid.NewGuid().ToString();
            ConcurrencyStamp = Guid.NewGuid().ToString();
            Status = Enums.Status.New;
        }

        [Column("user_id")]
        public override int Id { get; set; }

        [NotMapped]
        public int UserId => Id;

        [NotMapped]
        public string? DisplayName { get; set; }
        
        [NotMapped]
        public string? ProfilePictureDataUrl { get; set; }
        
        [NotMapped]
        public bool IsActive { get; set; }
        
        [NotMapped]
        public bool IsLive { get; set; }
        
        [NotMapped]
        public string? RefreshToken { get; set; }
        
        [NotMapped]
        public DateTime RefreshTokenExpiryTime { get; set; }        

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("user_name")]
        [StringLength(256)]
        public override string UserName { get; set; }

        [Column("normalized_user_name")]
        [StringLength(256)]
        public override string NormalizedUserName { get; set; }

        [Column("email")]
        [StringLength(256)]
        public override string Email { get; set; }

        [Column("normalized_email")]
        [StringLength(256)]
        public override string NormalizedEmail { get; set; }

        [Column("email_confirmed")]
        public new bool? EmailConfirmed { get; set; }

        [Column("password_hash")]
        public override string? PasswordHash { get; set; }

        [Column("security_stamp")]
        public override string SecurityStamp { get; set; }

        [Column("concurrency_stamp")]
        public override string ConcurrencyStamp { get; set; }

        [Column("phone_number")]
        public override string? PhoneNumber { get; set; }

        [Column("profile_about")]
        public string? ProfileAbout { get; set; }

        [Column("phone_number_confirmed")]
        public new bool? PhoneNumberConfirmed { get; set; }

        [Column("two_factor_enabled")]
        public new bool? TwoFactorEnabled { get; set; }

        [Column("lockout_end", TypeName = "timestamp with time zone")]
        public new Instant? LockoutEnd { get; set; }

        [Column("lockout_enabled")]
        public new bool? LockoutEnabled { get; set; }

        [Column("access_failed_count")]
        public override int AccessFailedCount { get; set; }

        [Column("tags")]
        public string?[]? Tags { get; set; }

        [Column("is_public")]
        public bool? IsPublic { get; set; }

        [Column("created_date", TypeName = "timestamp with time zone")]
        public Instant? CreatedDate { get; set; }

        [Column("modified_date", TypeName = "timestamp with time zone")]
        public Instant? ModifiedDate { get; set; }

        [Column("modified_user_id")]
        public int? ModifiedUserId { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.InverseModifiedUser))]
        public virtual User? ModifiedUser { get; set; }

        [Column("status")]
        public Status Status { get; set; }

        [Column("last_authenticate_date", TypeName = "timestamp with time zone")]
        public Instant? LastAuthenticateDate { get; set; }

        [Column("successful_authenticate_count")]
        public int? SuccessfulAuthenticateCount { get; set; }

        [InverseProperty("User")]
        public virtual Collection Collection { get; set; }

        [InverseProperty(nameof(ApiApplication.CreatedUser))]
        public virtual ICollection<ApiApplication> ApiApplicationCreatedUser { get; set; }

        [InverseProperty(nameof(ApiApplication.ModifiedUser))]
        public virtual ICollection<ApiApplication> ApiApplicationModifiedUser { get; set; }

        [InverseProperty(nameof(ApiApplication.ReviewedUser))]
        public virtual ICollection<ApiApplication> ApiApplicationReviewedUser { get; set; }

        [InverseProperty(nameof(FranchiseCategory.CreatedUser))]
        public virtual ICollection<FranchiseCategory> FranchiseCategoryCreatedUser { get; set; }

        [InverseProperty(nameof(FranchiseCategory.ModifiedUser))]
        public virtual ICollection<FranchiseCategory> FranchiseCategoryModifiedUser { get; set; }

        [InverseProperty(nameof(FranchiseCategory.ReviewedUser))]
        public virtual ICollection<FranchiseCategory> FranchiseCategoryReviewedUser { get; set; }

        [InverseProperty(nameof(Franchise.CreatedUser))]
        public virtual ICollection<Franchise> FranchiseCreatedUser { get; set; }

        [InverseProperty(nameof(Franchise.ModifiedUser))]
        public virtual ICollection<Franchise> FranchiseModifiedUser { get; set; }

        [InverseProperty(nameof(Franchise.ReviewedUser))]
        public virtual ICollection<Franchise> FranchiseReviewedUser { get; set; }

        [InverseProperty(nameof(Genre.CreatedUser))]
        public virtual ICollection<Genre> GenreCreatedUser { get; set; }

        [InverseProperty(nameof(Genre.ModifiedUser))]
        public virtual ICollection<Genre> GenreModifiedUser { get; set; }

        [InverseProperty(nameof(Genre.ReviewedUser))]
        public virtual ICollection<Genre> GenreReviewedUser { get; set; }

        [InverseProperty(nameof(Grade.CreatedUser))]
        public virtual ICollection<Grade> GradeCreatedUser { get; set; }

        [InverseProperty(nameof(Grade.ModifiedUser))]
        public virtual ICollection<Grade> GradeModifiedUser { get; set; }

        [InverseProperty(nameof(Grade.ReviewedUser))]
        public virtual ICollection<Grade> GradeReviewedUser { get; set; }

        [InverseProperty(nameof(GradeTerm.CreatedUser))]
        public virtual ICollection<GradeTerm> GradeTermCreatedUser { get; set; }

        [InverseProperty(nameof(GradeTerm.ModifiedUser))]
        public virtual ICollection<GradeTerm> GradeTermModifiedUser { get; set; }

        [InverseProperty(nameof(GradeTerm.ReviewedUser))]
        public virtual ICollection<GradeTerm> GradeTermReviewedUser { get; set; }

        [InverseProperty(nameof(User.ModifiedUser))]
        public virtual ICollection<User> InverseModifiedUser { get; set; }

        [InverseProperty(nameof(Issue.CreatedUser))]
        public virtual ICollection<Issue> IssueCreatedUser { get; set; }

        [InverseProperty(nameof(Issue.ModifiedUser))]
        public virtual ICollection<Issue> IssueModifiedUser { get; set; }

        [InverseProperty(nameof(Issue.ReviewedUser))]
        public virtual ICollection<Issue> IssueReviewedUser { get; set; }

        [InverseProperty(nameof(IssueType.CreatedUser))]
        public virtual ICollection<IssueType> IssueTypeCreatedUser { get; set; }

        [InverseProperty(nameof(IssueType.ModifiedUser))]
        public virtual ICollection<IssueType> IssueTypeModifiedUser { get; set; }

        [InverseProperty(nameof(IssueType.ReviewedUser))]
        public virtual ICollection<IssueType> IssueTypeReviewedUser { get; set; }

        [InverseProperty(nameof(PublisherCategory.CreatedUser))]
        public virtual ICollection<PublisherCategory> PublisherCategoryCreatedUser { get; set; }

        [InverseProperty(nameof(PublisherCategory.ModifiedUser))]
        public virtual ICollection<PublisherCategory> PublisherCategoryModifiedUser { get; set; }

        [InverseProperty(nameof(PublisherCategory.ReviewedUser))]
        public virtual ICollection<PublisherCategory> PublisherCategoryReviewedUser { get; set; }

        [InverseProperty(nameof(Publisher.CreatedUser))]
        public virtual ICollection<Publisher> PublisherCreatedUser { get; set; }

        [InverseProperty(nameof(Publisher.ModifiedUser))]
        public virtual ICollection<Publisher> PublisherModifiedUser { get; set; }

        [InverseProperty(nameof(Publisher.ReviewedUser))]
        public virtual ICollection<Publisher> PublisherReviewedUser { get; set; }

        [InverseProperty(nameof(SeriesCategory.CreatedUser))]
        public virtual ICollection<SeriesCategory> SeriesCategoryCreatedUser { get; set; }

        [InverseProperty(nameof(SeriesCategory.ModifiedUser))]
        public virtual ICollection<SeriesCategory> SeriesCategoryModifiedUser { get; set; }

        [InverseProperty(nameof(SeriesCategory.ReviewedUser))]
        public virtual ICollection<SeriesCategory> SeriesCategoryReviewedUser { get; set; }

        [InverseProperty(nameof(Series.CreatedUser))]
        public virtual ICollection<Series> SeriesCreatedUser { get; set; }

        [InverseProperty(nameof(Series.ModifiedUser))]
        public virtual ICollection<Series> SeriesModifiedUser { get; set; }

        [InverseProperty(nameof(Series.ReviewedUser))]
        public virtual ICollection<Series> SeriesReviewedUser { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserLogin> UserLogin { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserToken> UserToken { get; set; }

        [NotMapped]
        public virtual ICollection<UserUserRole> UserRoles
        {
            get => UserUserRole;
            set
            {
                UserUserRole = value;
                Roles = value?.Select(x => x.UserRole).Select(x => x.NormalizedName).Distinct().ToArray();
            }
        }

        [NotMapped] public string?[]? AssignedRoles => UserRoles?.Select(x => x.UserRole?.Name).ToArray();

        [InverseProperty("User")]
        public virtual ICollection<UserUserRole> UserUserRole { get; set; }

        [NotMapped]
        public IEnumerable<string> Roles { get; set; }
        
        public override string ToString() => $"Id [{ Id }] UserName [{ UserName}]";
    }
}