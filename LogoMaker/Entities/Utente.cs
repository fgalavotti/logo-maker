
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LogoMaker.Entities;

[Table("usertable")]
public class Utente
{
    [Key]
    [Column("Username")]
    [Required]
    [RegularExpression(@"^[A-Z][a-z]+$")]
    public required string Username{ get; set; }

    [Column("Password")]
    [Required]
    public required string Password{ get; set; }

    [Column("Gender")]
    [Required]
    public required string Gender{ get; set; }

    [Column("Ruolo")]
    [Required]
    public required string Role{ get; set; }

    public List<Società>? SocietàUtente{ get; set; }
}