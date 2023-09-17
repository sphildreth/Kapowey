using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("publisher_category")]
    public partial class PublisherCategory : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => PublisherCategoryId;
            set => PublisherCategoryId = value;
        }  
        
        [Key]
        [Column("publisher_category_id")]
        public int PublisherCategoryId { get; set; }

        [Column("parent_publisher_category_id")]
        public int? ParentPublisherCategoryId { get; set; }

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

        [Column("status")]
        public Status Status { get; set; }

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

        [ForeignKey(nameof(CreatedUserId))]
        [InverseProperty(nameof(User.PublisherCategoryCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.PublisherCategoryModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.PublisherCategoryReviewedUser))]
        public virtual User ReviewedUser { get; set; }
    }
}