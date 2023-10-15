using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kapowey.Core.Entities;

[Table("user_claim")]
public class UserClaim : IdentityUserClaim<int>, IEntity<int>
{
    [Key]
    [Column("user_claim_id")]
    public override int Id { get; set; }
    
    [Column("user_id")]
    public override int UserId { get; set; }

    [Column("claim_type")]
    [StringLength(128)]
    public override string? ClaimType { get; set; }

    [Column("claim_value")]
    [StringLength(128)]
    public override string? ClaimValue { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Claims")]
    public virtual User User { get; set; }
}