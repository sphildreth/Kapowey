using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kapowey.Core.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kapowey.Core.Entities;

[Table("user_role_claim")]
public class UserRoleClaim : IdentityRoleClaim<int>, IEntity<int>
{
    [Key]
    [Column("user_role_claim_id")]
    public override int Id { get; set; }
    
    [Column("user_role_id")]
    public override int RoleId { get; set; }

    [Key]
    [Column("claim_type")]
    [StringLength(128)]
    public override string? ClaimType { get; set; }

    [Key]
    [Column("claim_value")]
    [StringLength(128)]
    public override string? ClaimValue { get; set; }

    [ForeignKey(nameof(RoleId))]
    [InverseProperty("Claims")]
    public virtual UserRole Role { get; set; }
}