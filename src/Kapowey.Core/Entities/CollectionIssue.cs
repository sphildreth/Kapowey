using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("collection_issue")]
    public partial class CollectionIssue : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => CollectionIssueId;
            set => CollectionIssueId = value;
        }           
        
        public CollectionIssue()
        {
            CollectionIssueGradeTerm = new HashSet<CollectionIssueGradeTerm>();
        }

        [Key]
        [Column("collection_issue_id")]
        public int CollectionIssueId { get; set; }

        [Column("collection_id")]
        public int? CollectionId { get; set; }

        [Column("issue_id")]
        public int? IssueId { get; set; }

        [Column("grade_id")]
        public int? GradeId { get; set; }

        [Column("sort_order")]
        public int? SortOrder { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("notes")]
        public string Notes { get; set; }

        [Column("tags")]
        public string[] Tags { get; set; }

        [Column("number_of_copies_owned")]
        public int? NumberOfCopiesOwned { get; set; }

        [Column("is_digital")]
        public bool? IsDigital { get; set; }

        [Column("is_wanted")]
        public bool? IsWanted { get; set; }

        [Column("is_public")]
        public bool? IsPublic { get; set; }

        [Column("has_read")]
        public bool? HasRead { get; set; }

        [Column("is_for_sale")]
        public bool? IsForSale { get; set; }

        [Column("price_paid", TypeName = "numeric(12,2)")]
        public decimal? PricePaid { get; set; }

        [Column("acquisition_date", TypeName = "timestamp with time zone")]
        public Instant? AcquisitionDate { get; set; }

        [Column("last_activity", TypeName = "timestamp with time zone")]
        public Instant? LastActivity { get; set; }

        [Column("created_date", TypeName = "timestamp with time zone")]
        public Instant? CreatedDate { get; set; }

        [Column("status")]
        public Status Status { get; set; }

        [Column("rating")]
        public Rating Rating { get; set; }

        [ForeignKey(nameof(CollectionId))]
        [InverseProperty("CollectionIssue")]
        public virtual Collection Collection { get; set; }

        [ForeignKey(nameof(GradeId))]
        [InverseProperty("CollectionIssue")]
        public virtual Grade Grade { get; set; }

        [ForeignKey(nameof(IssueId))]
        [InverseProperty("CollectionIssue")]
        public virtual Issue Issue { get; set; }

        [InverseProperty("CollectionIssue")]
        public virtual ICollection<CollectionIssueGradeTerm> CollectionIssueGradeTerm { get; set; }
    }
}