using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("publisher")]
    public partial class Publisher : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => PublisherId;
            set => PublisherId = value;
        }  
        
        public Publisher()
        {
            Franchise = new HashSet<Franchise>();
        }

        [Key]
        [Column("publisher_id")]
        public int PublisherId { get; set; }

        [Column("parent_publisher_id")]
        public int? ParentPublisherId { get; set; }

        [Column("publisher_category_id")]
        public int? PublisherCategoryId { get; set; }

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
        [StringLength(10)]
        public string ShortName { get; set; }

        [Column("year_began")]
        public int? YearBegan { get; set; }

        [Column("year_end")]
        public int? YearEnd { get; set; }

        [Required]
        [Column("country_code")]
        [StringLength(3)]
        public string CountryCode { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("url")]
        [StringLength(1000)]
        public string Url { get; set; }

        [Column("tags")]
        public string[] Tags { get; set; }

        [Column("franchise_count")]
        public int FranchiseCount { get; set; }

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
        [InverseProperty(nameof(User.PublisherCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.PublisherModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.PublisherReviewedUser))]
        public virtual User ReviewedUser { get; set; }

        [InverseProperty("Publisher")]
        public virtual ICollection<Franchise> Franchise { get; set; }
    }
}