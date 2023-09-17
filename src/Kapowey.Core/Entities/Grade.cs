﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Enums;
using NodaTime;

namespace Kapowey.Core.Entities
{
    [Table("grade")]
    public partial class Grade : IEntity<int>
    {
        [NotMapped]
        public int Id
        {
            get => GradeId;
            set => GradeId = value;
        }  
        
        public Grade()
        {
            CollectionIssue = new HashSet<CollectionIssue>();
        }

        [Key]
        [Column("grade_id")]
        public int GradeId { get; set; }

        [Column("scale", TypeName = "numeric(3,1)")]
        public decimal? Scale { get; set; }

        [Column("sort_order")]
        public int? SortOrder { get; set; }

        [Required]
        [Column("name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [Column("abbreviation")]
        [StringLength(6)]
        public string Abbreviation { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("api_key")]
        public Guid? ApiKey { get; set; }

        [Column("notes")]
        public string Notes { get; set; }

        [Column("tags")]
        public string[] Tags { get; set; }

        [Column("is_basic_grade")]
        public bool? IsBasicGrade { get; set; }

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
        [InverseProperty(nameof(User.GradeCreatedUser))]
        public virtual User CreatedUser { get; set; }

        [ForeignKey(nameof(ModifiedUserId))]
        [InverseProperty(nameof(User.GradeModifiedUser))]
        public virtual User ModifiedUser { get; set; }

        [ForeignKey(nameof(ReviewedUserId))]
        [InverseProperty(nameof(User.GradeReviewedUser))]
        public virtual User ReviewedUser { get; set; }

        [InverseProperty("Grade")]
        public virtual ICollection<CollectionIssue> CollectionIssue { get; set; }
    }
}