using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("franchise")]
    public partial class Franchise : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => FranchiseId;
            set => FranchiseId = value;
        }    
        
        public Franchise()
        {
            Series = new HashSet<Series>();
        }

        [Key]
        [Column("franchise_id")]
        public int FranchiseId { get; set; }

        [Column("publisher_id")]
        public int? PublisherId { get; set; }

        [Column("parent_franchise_id")]
        public int? ParentFranchiseId { get; set; }

        [Column("franchise_category_id")]
        public int? FranchiseCategoryId { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("gcd_id")]
        public int? GcdId { get; set; }

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

        [Column("description")]
        public string Description { get; set; }

        [Column("url")]
        [StringLength(1000)]
        public string Url { get; set; }

        [Column("tags")]
        public string[] Tags { get; set; }

        [Column("series_count")]
        public int SeriesCount { get; set; }

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

        [ForeignKey(nameof(CreatedUserId))]
        [InverseProperty(nameof(User.FranchiseCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.FranchiseModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(PublisherId))]
        [InverseProperty("Franchise")]
        public virtual Publisher Publisher { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.FranchiseReviewedUser))]
        public virtual User ReviewedUser { get; set; }

        [InverseProperty("Franchise")]
        public virtual ICollection<Series> Series { get; set; }
    }
}