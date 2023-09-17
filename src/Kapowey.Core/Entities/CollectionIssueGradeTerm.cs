using System.ComponentModel.DataAnnotations.Schema;

namespace Kapowey.Core.Entities
{
    [Table("collection_issue_grade_term")]
    public partial class CollectionIssueGradeTerm
    {
        [Column("collection_issue_id")]
        public int CollectionIssueId { get; set; }

        [Column("grade_term_id")]
        public int GradeTermId { get; set; }

        [ForeignKey(nameof(CollectionIssueId))]
        [InverseProperty("CollectionIssueGradeTerm")]
        public virtual CollectionIssue CollectionIssue { get; set; }

        [ForeignKey(nameof(GradeTermId))]
        [InverseProperty("CollectionIssueGradeTerm")]
        public virtual GradeTerm GradeTerm { get; set; }
    }
}