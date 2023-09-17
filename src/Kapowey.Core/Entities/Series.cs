using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("series")]
    public partial class Series : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => SeriesId;
            set => SeriesId = value;
        } 
        
        public Series()
        {
            Issue = new HashSet<Issue>();
        }

        [Key]
        [Column("series_id")]
        public int SeriesId { get; set; }

        [Column("franchise_id")]
        public int? FranchiseId { get; set; }

        [Column("series_category_id")]
        public int? SeriesCategoryId { get; set; }

        [Column("genre_id")]
        public int? GenreId { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("gcd_id")]
        public int? GcdId { get; set; }

        [Column("first_issue_id")]
        public int? FirstIssueId { get; set; }

        [Column("last_issue_id")]
        public int? LastIssueId { get; set; }

        [Required]
        [Column("name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [Column("short_name")]
        [StringLength(20)]
        public string ShortName { get; set; }

        [Column("year_began")]
        public int? YearBegan { get; set; }

        [Column("year_end")]
        public int? YearEnd { get; set; }

        [Required]
        [Column("culture_code")]
        [StringLength(2)]
        public string CultureCode { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("url")]
        [StringLength(1000)]
        public string Url { get; set; }

        [Column("tags")]
        public string[] Tags { get; set; }

        [Column("issue_count")]
        public int IssueCount { get; set; }

        [Column("created_date", TypeName = "timestamp with time zone")]
        public Instant? CreatedDate { get; set; }

        [Column("created_user_id")]
        public int? CreatedUserId { get; set; }

        [Column("modified_date", TypeName = "timestamp with time zone")]
        public Instant? ModifiedDate { get; set; }

        [Column("modified_user_id")]
        public int? ModifiedUserId { get; set; }

        [Column("reviewed_date", TypeName = "timestamp with time zone")]
        public Instant? ReviewedDate { get; set; }

        [Column("reviewed_user_id")]
        public int? ReviewedUserId { get; set; }

        [Column("status")]
        public Status Status { get; set; }

        [Column("rating")]
        public Rating Rating { get; set; }

        [ForeignKey(nameof(CreatedUserId))]
        [InverseProperty(nameof(User.SeriesCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(FranchiseId))]
        [InverseProperty("Series")]
        public virtual Franchise Franchise { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.SeriesModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.SeriesReviewedUser))]
        public virtual User ReviewedUser { get; set; }

        [InverseProperty("Series")]
        public virtual ICollection<Issue> Issue { get; set; }
    }
}