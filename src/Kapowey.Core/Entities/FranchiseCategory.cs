using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("franchise_category")]
    public partial class FranchiseCategory : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => FranchiseCategoryId;
            set => FranchiseCategoryId = value;
        }          
        
        [Key]
        [Column("franchise_category_id")]
        public int FranchiseCategoryId { get; set; }

        [Column("parent_franchise_category_id")]
        public int? ParentFranchiseCategoryId { get; set; }

        [Required]
        [Column("name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [Column("short_name")]
        [StringLength(10)]
        public string ShortName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("url")]
        [StringLength(1000)]
        public string Url { get; set; }

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
        [InverseProperty(nameof(User.FranchiseCategoryCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.FranchiseCategoryModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.FranchiseCategoryReviewedUser))]
        public virtual User ReviewedUser { get; set; }
    }
}