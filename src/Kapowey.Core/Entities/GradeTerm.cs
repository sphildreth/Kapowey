using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("grade_term")]
    public partial class GradeTerm : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => GradeTermId;
            set => GradeTermId = value;
        }  
        
        public GradeTerm()
        {
            CollectionIssueGradeTerm = new HashSet<CollectionIssueGradeTerm>();
        }

        [Key]
        [Column("grade_term_id")]
        public int GradeTermId { get; set; }

        [Column("sort_order")]
        public int? SortOrder { get; set; }

        [Required]
        [Column("name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("tags")]
        public string[] Tags { get; set; }

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
        [InverseProperty(nameof(User.GradeTermCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.GradeTermModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.GradeTermReviewedUser))]
        public virtual User ReviewedUser { get; set; }

        [InverseProperty("GradeTerm")]
        public virtual ICollection<CollectionIssueGradeTerm> CollectionIssueGradeTerm { get; set; }
    }
}