using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("issue")]
    public partial class Issue : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => IssueId;
            set => IssueId = value;
        }  
        
        [Key]
        [Column("issue_id")]
        public int IssueId { get; set; }

        [Column("series_id")]
        public int? SeriesId { get; set; }

        [Column("issuetype_id")]
        public int? IssueTypeId { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("gcd_id")]
        public int? GcdId { get; set; }

        [Column("reprint_of_issue_id")]
        public int? ReprintOfIssueId { get; set; }

        [Column("sort_order")]
        public int? SortOrder { get; set; }

        [Column("number")]
        [StringLength(10)]
        public string Number { get; set; }

        [Required]
        [Column("title")]
        [StringLength(500)]
        public string Title { get; set; }

        [Column("variant_title")]
        [StringLength(500)]
        public string VariantTitle { get; set; }

        [Column("short_title")]
        [StringLength(20)]
        public string ShortTitle { get; set; }

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

        [Column("key_date", TypeName = "timestamp with time zone")]
        public DateTime KeyDate { get; set; }

        [Column("isbn")]
        [StringLength(25)]
        public string Isbn { get; set; }

        [Column("cover_price", TypeName = "numeric(12,2)")]
        public decimal? CoverPrice { get; set; }

        [Column("barcode")]
        [StringLength(25)]
        public string Barcode { get; set; }

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
        [InverseProperty(nameof(User.IssueCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(IssueTypeId))]
        [InverseProperty("Issue")]
        public virtual IssueType IssueType { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.IssueModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.IssueReviewedUser))]
        public virtual User ReviewedUser { get; set; }

        [ForeignKey(nameof(SeriesId))]
        [InverseProperty("Issue")]
        public virtual Series Series { get; set; }

        [InverseProperty("Issue")]
        public virtual CollectionIssue CollectionIssue { get; set; }
    }
}